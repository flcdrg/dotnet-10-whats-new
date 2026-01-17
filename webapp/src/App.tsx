import { useEffect, useMemo, useState } from 'react'
import './App.css'

interface Accessory {
  id: number
  name: string
  description?: string | null
  category: string
  price: number
  stockQuantity: number
  imageUrl?: string | null
}

type FetchState = 'idle' | 'loading' | 'error' | 'success'

const defaultApiBase = 'http://localhost:5148'

function App() {
  const apiBase = useMemo(
    () => import.meta.env.VITE_API_BASE_URL ?? defaultApiBase,
    []
  )

  const [accessories, setAccessories] = useState<Accessory[]>([])
  const [state, setState] = useState<FetchState>('idle')
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()
    const load = async () => {
      setState('loading')
      setError(null)
      try {
        const response = await fetch(`${apiBase}/api/accessories`, {
          signal: controller.signal,
        })

        if (!response.ok) {
          throw new Error(`Request failed with status ${response.status}`)
        }

        const data = (await response.json()) as Accessory[]
        setAccessories(data)
        setState('success')
      } catch (err) {
        if (controller.signal.aborted) return
        const message = err instanceof Error ? err.message : 'Unknown error'
        setError(message)
        setState('error')
      }
    }

    void load()

    return () => controller.abort()
  }, [apiBase])

  return (
    <main className="page">
      <header className="hero">
        <div>
          <p className="eyebrow">Pet Store Accessories</p>
          <h1>Find the right accessory for your pet</h1>
          <p className="lede">
            Browse our curated list of collars, toys, grooming tools, and travel
            gear. These items load directly from the backend API.
          </p>
        </div>
      </header>

      <section className="panel">
        {state === 'loading' && <p className="status">Loading accessories…</p>}
        {state === 'error' && error && (
          <p className="status error">Unable to load accessories: {error}</p>
        )}

        {state === 'success' && accessories.length === 0 && (
          <p className="status">No accessories found.</p>
        )}

        {state === 'success' && accessories.length > 0 && (
          <div className="grid">
            {accessories.map((item) => (
              <article key={item.id} className="card">
                <div className="card__body">
                  <p className="badge">{item.category}</p>
                  <h2>{item.name}</h2>
                  {item.description && <p className="muted">{item.description}</p>}
                </div>
                <div className="card__meta">
                  <span className="price">
                    {item.price.toLocaleString(undefined, {
                      style: 'currency',
                      currency: 'USD',
                      minimumFractionDigits: 2,
                    })}
                  </span>
                  <span className="stock">In stock: {item.stockQuantity}</span>
                </div>
              </article>
            ))}
          </div>
        )}
      </section>
    </main>
  )
}

export default App
