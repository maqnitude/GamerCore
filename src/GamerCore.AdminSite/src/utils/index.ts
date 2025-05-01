export function formatDate(value: Date | string): string {
  const date = typeof value === "string" ? new Date(value) : value;

  return new Intl.DateTimeFormat("en-GB", {
    dateStyle: "short",
    timeStyle: "short"

    // day: "2-digit",
    // month: "2-digit",
    // year: "numeric",
    // hour: "2-digit",
    // minute: "2-digit",
    // hour12: false
  }).format(date);
}