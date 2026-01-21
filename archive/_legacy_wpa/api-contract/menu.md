
# Menu API

**Base Path:** `/api/menu`

## 1. Get Categories
Retrieves the root menu categories.

- **Method:** `GET`
- **Path:** `/categories`

### Response Body
```typescript
interface MenuCategory {
  id: string;
  name: string;
  subcategories?: MenuCategory[];
}
[];
```

---

## 2. Get Items
Retrieves menu items for a specific category.

- **Method:** `GET`
- **Path:** `/items`

### Query Parameters
- `categoryId` (required): The ID of the category.

### Response Body
```typescript
interface MenuItem {
  id: string;
  name: string;
  price: number;
  description?: string;
  categoryId: string;
  stockQuantity?: number; // Optional tracking
}
[];
```

---

## 3. Search Items
Searches for menu items by name or metadata.

- **Method:** `GET`
- **Path:** `/items/search`

### Query Parameters
- `q` (required): Search query string.

### Response Body
Returns `MenuItem[]`.

---

## 4. Get Item Modifiers
Retrieves available modifier groups for a specific menu item.

- **Method:** `GET`
- **Path:** `/items/{menuItemId}/modifiers`

### Response Body
```typescript
interface ModifierOption {
  id: string;
  name: string;
  priceDelta: number;
}

interface ModifierGroup {
  id: string;
  name: string;
  minSelection: number;
  maxSelection: number;
  options: ModifierOption[];
}
[];
```
