# Agent Instructions for webapp

This is a Vite React 19 web application that provides the user-facing interface for the pet store accessories application.

## Project Overview

- **Framework**: React 19 with TypeScript 5.9
- **Build Tool**: Vite 7.2.4
- **Purpose**: Customer-facing web interface
- **Port**: <http://localhost:5173> (default Vite dev server)
- **API Base URL**: <http://localhost:5148> (backend API)
- **Node Version**: 24.x (required)
- **Package Manager**: pnpm 10.28.0+

## Technology Stack

- **UI Framework**: React 19 with React DOM
- **Language**: TypeScript 5.9 with strict mode enabled
- **Build Tool**: Vite 7 with Hot Module Replacement (HMR)
- **Code Quality**:
  - ESLint 9 with TypeScript ESLint
  - React Hooks linting
  - React Refresh for fast refresh
- **Styling**: CSS (currently using vanilla CSS)

## Project Structure

```plaintext
webapp/
├── src/
│   ├── main.tsx           # Application entry point
│   ├── App.tsx            # Root component
│   ├── App.css            # App-specific styles
│   ├── index.css          # Global styles
│   └── assets/            # Static assets (images, etc.)
├── public/                # Public static files
├── index.html             # HTML template
├── vite.config.ts         # Vite configuration
├── tsconfig.json          # TypeScript project references
├── tsconfig.app.json      # App TypeScript config
├── tsconfig.node.json     # Node/build TypeScript config
├── eslint.config.js       # ESLint configuration
├── package.json           # Dependencies and scripts
└── pnpm-lock.yaml         # Lock file for reproducible installs
```

## Running the Application

### Install Dependencies

```bash
cd webapp
pnpm install
```

### Development Server

```bash
cd webapp
pnpm dev
```

Server runs on <http://localhost:5173> with HMR enabled.

### Build for Production

```bash
cd webapp
pnpm build
```

Output in `dist/` folder.

### Preview Production Build

```bash
cd webapp
pnpm preview
```

### Lint Code

```bash
cd webapp
pnpm lint
```

## Development Guidelines

1. **Component Development**:
   - Use functional components with hooks
   - Place components in `src/` or organize into subdirectories
   - Use `.tsx` extension for components with JSX
   - Follow React 19 best practices (use new hooks, concurrent features)

2. **TypeScript**:
   - Strict mode enabled (all type checks enforced)
   - Use explicit types for props and state
   - Avoid `any` type; use `unknown` if type is truly unknown
   - No unused locals or parameters (enforced by compiler)

3. **Styling**:
   - Currently using CSS files
   - Import CSS directly into components
   - Consider CSS Modules for component-scoped styles
   - Can add Tailwind CSS, styled-components, or other solutions

4. **API Integration**:
   - Backend API is at <http://localhost:5148>
   - Use `fetch` or add a library like `axios` or `@tanstack/react-query`
   - Handle loading states and errors appropriately
   - Implement proper CORS handling (backend must allow origin)

5. **State Management**:
   - Start with React's built-in hooks (`useState`, `useReducer`, `useContext`)
   - Add Redux, Zustand, or Jotai if global state becomes complex
   - Consider `@tanstack/react-query` for server state

6. **Code Quality**:
   - Run `pnpm lint` before committing
   - Fix all ESLint warnings and errors
   - Follow React Hooks rules (enforced by ESLint)
   - Use React Refresh patterns for fast refresh

## Common Patterns

### Creating a New Component

```tsx
// src/components/ProductCard.tsx
interface ProductCardProps {
  name: string;
  price: number;
  onAddToCart: () => void;
}

export function ProductCard({ name, price, onAddToCart }: ProductCardProps) {
  return (
    <div className="product-card">
      <h3>{name}</h3>
      <p>${price}</p>
      <button onClick={onAddToCart}>Add to Cart</button>
    </div>
  );
}
```

### API Call Example

```tsx
import { useState, useEffect } from 'react';

interface Product {
  id: number;
  name: string;
  price: number;
}

export function ProductList() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch('http://localhost:5148/api/products')
      .then(res => res.json())
      .then(data => {
        setProducts(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <div>Loading...</div>;
  if (error) return <div>Error: {error}</div>;

  return (
    <div>
      {products.map(product => (
        <div key={product.id}>{product.name}</div>
      ))}
    </div>
  );
}
```

### Using Context for Global State

```tsx
import { createContext, useContext, useState, ReactNode } from 'react';

interface CartContextValue {
  items: number;
  addItem: () => void;
}

const CartContext = createContext<CartContextValue | undefined>(undefined);

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState(0);
  
  const addItem = () => setItems(prev => prev + 1);

  return (
    <CartContext.Provider value={{ items, addItem }}>
      {children}
    </CartContext.Provider>
  );
}

export function useCart() {
  const context = useContext(CartContext);
  if (!context) throw new Error('useCart must be used within CartProvider');
  return context;
}
```

## Configuration

### Vite Config

- Default configuration with React plugin
- Can add proxy for API calls, env variables, plugins

### TypeScript Config

- **tsconfig.app.json**: Strict mode, ES2022 target, React JSX
- **tsconfig.node.json**: For Vite config and build scripts

### ESLint Config

- Flat config format (ESLint 9)
- React Hooks rules enforced
- TypeScript ESLint recommended rules
- React Refresh rules for HMR

## Dependencies

### Runtime

- `react` ^19.2.0 - UI library
- `react-dom` ^19.2.0 - React DOM rendering

### Development

- `vite` ^7.2.4 - Build tool
- `typescript` ~5.9.3 - Type checking
- `@vitejs/plugin-react` - Vite React support
- `eslint` ^9.39.1 - Linting
- `typescript-eslint` - TypeScript linting rules

## Integration Points

- **backend**: Consumes REST API at <http://localhost:5148>
- **function**: May call Azure Function HTTP endpoints directly

## Notes for AI Agents

- This uses React 19 (latest features available)
- Vite 7 for blazing fast HMR and builds
- TypeScript strict mode is enforced - all types must be explicit
- ESLint flat config format (v9), not legacy `.eslintrc`
- Use `pnpm` as package manager (not npm or yarn)
- Node 24.x is required (specified in AGENTS.md and likely .nvmrc)
- All React components should be functional, not class-based
- Follow React Hooks rules and best practices
- Use StrictMode for development (already enabled in main.tsx)
- Backend CORS must allow <http://localhost:5173> for API calls to work
