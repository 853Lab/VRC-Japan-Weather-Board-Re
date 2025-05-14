
import { parseArgs } from "jsr:@std/cli/parse-args"
import { dateFormat, FormatWithTimezoneLite } from "./method.ts"
import * as fs from "jsr:@std/fs"
import * as path from "jsr:@std/path"

let client: Deno.HttpClient | undefined

const flags = parseArgs(Deno.args, {
  string: [
    "proxy",
  ],
})

if (flags.proxy) {
  client = Deno.createHttpClient({
    proxy: {
      url: flags.proxy,
    },
  })
}

const oneDay = 86400000

const weathers: Record<string, Date> = {
  day0: new Date(),
  day1: new Date(Date.now() + oneDay),
  day2: new Date(Date.now() + oneDay * 2),
}

const getWeather = async (date: Date = new Date(), client?: Deno.HttpClient) => {
  const dateString = dateFormat("YYYYmmdd", date)
  const url = `https://site.weathernews.jp/site/forecast/image/map/JAPAN.${dateString}.png`

  let res: Response
  if (client) {
    res = await fetch(url, {
      method: "GET",
      client,
    })
  }
  else {
    res = await fetch(url, {
      method: "GET",
    })
  }

  return res
}
const folder = path.join(".", "pages")
if (!await fs.exists(folder)) {
  await Deno.mkdir(folder, { recursive: true })
}
for (const [key, value] of Object.entries(weathers)) {
  const res = await getWeather(value, client)
  if (res.body) {
    const filepath = path.join(folder, `${key}.png`)
    await Deno.writeFile(filepath, res.body)
  }
}

// 保活
const timeNow = new Date()
const thisDay = timeNow.getDate()

if (thisDay === 1 || thisDay === 15) {
  if (await fs.exists("updatetime.txt")) {
    await Deno.remove("updatetime.txt")
  }
  await Deno.writeTextFile("updatetime.txt", FormatWithTimezoneLite(timeNow, "+09:00"))
}
