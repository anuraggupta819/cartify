// Vibration API support is mobile-only (Android Chrome family) and always absent on iOS
// Safari/desktop — guarded so this is a harmless no-op everywhere else.
export function triggerHaptic(pattern: number | number[] = 15) {
  if (typeof navigator !== 'undefined' && 'vibrate' in navigator) {
    navigator.vibrate(pattern)
  }
}
