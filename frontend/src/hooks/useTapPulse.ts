import { useRef, useState } from 'react'

// Visual stand-in for haptic feedback on platforms that can't vibrate (iOS Safari has
// never implemented the Vibration API) — a brief scale-down-and-back pulse.
export function useTapPulse(durationMs = 150) {
  const [isPulsing, setIsPulsing] = useState(false)
  const timeoutRef = useRef<ReturnType<typeof setTimeout>>(undefined)

  function pulse() {
    setIsPulsing(true)
    clearTimeout(timeoutRef.current)
    timeoutRef.current = setTimeout(() => setIsPulsing(false), durationMs)
  }

  return { isPulsing, pulse }
}
