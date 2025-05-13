/**
 * 
 * @param {string} fmt 
 * @param {Date} date 
 * @returns 
 */
export const dateFormat = (fmt: string, date?: Date | undefined) => {
  if (date === undefined) date = new Date()
  const hours = date.getHours()
  const opt = {
    "Y+": date.getFullYear().toString(),        // 年
    "m+": (date.getMonth() + 1).toString(),     // 月
    "d+": date.getDate().toString(),            // 日
    "H+": hours.toString(),                     // 24时
    "h+": ((hours > 12) ? hours - 12 : hours).toString(),  // 12时
    "M+": date.getMinutes().toString(),         // 分
    "S+": date.getSeconds().toString(),         // 秒
    "t+": ((hours >= 12) ? "下午" : "上午")      // 上下午
    // 有其他格式化字符需求可以继续添加，必须转化成字符串
  }
  let r: RegExpExecArray | null
  for (const k in opt) {
    if (isValidKey(k, opt)) {
      r = new RegExp("(" + k + ")").exec(fmt)
      if (r) {
        const o: string = opt[k]
        fmt = fmt.replace(r[1], (r[1].length == 1) ? (o) : (o.padStart(r[1].length, "0")))
      }
    }
  }
  return fmt
}
export const isValidKey = (key: string, obj: object): key is keyof typeof obj => {
  return Object.prototype.hasOwnProperty.call(obj, key)
}

export const FormatWithTimezoneLite = (date: Date, timezone: string): string => {
  const options: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    second: '2-digit',
    timeZone: timezone, // 使用输入的时区
    hourCycle: 'h23', // 保持 24 小时制
  }
  const parts = new Intl.DateTimeFormat('en-GB', options).formatToParts(date)

  // 拼接成 ISO8601 格式
  // const [year, month, day, hour, minute, second] = [
  const [year, month, day] = [
    // const [year, month, day, hour, minute] = [
    parts.find(p => p.type === 'year')?.value,
    parts.find(p => p.type === 'month')?.value,
    parts.find(p => p.type === 'day')?.value,
    parts.find(p => p.type === 'hour')?.value,
    parts.find(p => p.type === 'minute')?.value,
    parts.find(p => p.type === 'second')?.value,
  ]

  return `${year}-${month}-${day}`
  // return `${year}-${month}-${day}T${hour}:${minute}${timezone}`
}