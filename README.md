# Savage - Inventory Management System

Full-stack inventory management application with role-based access for internal admins and external vendors.

**Frontend**: React.js + Material UI (cyberpunk theme)
**Backend**: ASP.NET Core 8 Web API + MongoDB
**Auth**: JWT with BCrypt password hashing
**Target deployment**: AWS EC2 + Amazon DocumentDB

---

## Prerequisites

- **Node.js** >= 18 (tested with v20)
- **npm** >= 8
- **.NET SDK** 8.0
- **MongoDB** 7.0 (or Amazon DocumentDB for production)
- **Git**

### Install prerequisites (Ubuntu)

```bash
# Node.js 20 LTS
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs

# .NET 8 SDK
sudo apt-get install -y dotnet-sdk-8.0

# MongoDB 7.0
curl -fsSL https://www.mongodb.org/static/pgp/server-7.0.asc | sudo gpg -o /usr/share/keyrings/mongodb-server-7.0.gpg --dearmor
echo "deb [ signed-by=/usr/share/keyrings/mongodb-server-7.0.gpg ] https://repo.mongodb.org/apt/ubuntu jammy/mongodb-org/7.0 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-7.0.list
sudo apt-get update && sudo apt-get install -y mongodb-org
sudo systemctl start mongod && sudo systemctl enable mongod
```

---

## Quick Start (Recommended)

The easiest way to run Savage is with the included start script. It handles all dependency installation and launches both servers in separate tmux windows.

### Requirements

The following must be installed before running the script:

- **.NET SDK 8.0** -- `sudo apt-get install -y dotnet-sdk-8.0`
- **MongoDB 7.0** -- see [Install prerequisites](#install-prerequisites-ubuntu) above
- **tmux** -- `sudo apt-get install -y tmux`
- **curl** -- needed for the automatic nvm/Node.js installation

You do **not** need to install Node.js or nvm manually. The script installs nvm and Node 20 automatically if they are missing.

### Run

From the project root (`savage/` directory):

```bash
bash start.sh
```

The script will:

1. Install nvm and Node 20 (if not already present)
2. Verify .NET SDK and MongoDB are available
3. Start MongoDB if it is not running
4. Restore backend .NET dependencies and build the API
5. Install frontend npm dependencies
6. Launch the backend (port 5000) and frontend (port 3000) in separate tmux windows
7. Attach you to the tmux session

Once running, open http://localhost:3000 in your browser.

### tmux controls

- **Switch windows**: `Ctrl+B` then `N`
- **Detach** (servers keep running): `Ctrl+B` then `D`
- **Reattach**: `tmux attach -t savage`
- **Stop everything**: `tmux kill-session -t savage`

---

## Manual Setup

If you prefer to start each service yourself instead of using the start script:

### 1. Clone the repository

```bash
git clone https://github.com/fresh3nough/savage.git
cd savage
```

### 2. Backend setup

```bash
cd back/Savage.Api
dotnet restore
dotnet build
```

The backend auto-seeds the MongoDB database with fake data on first startup (5 vendors, 5 locations, 25 inventory items, 5 shipments, 4 user accounts).

### 3. Frontend setup

```bash
cd front
npm install
```

### 4. Start the backend (port 5000)

```bash
cd back/Savage.Api
dotnet run --urls "http://localhost:5000"
```

### 5. Start the frontend (port 3000)

In a separate terminal:

```bash
cd front
npm start
```

Open http://localhost:3000 in your browser.

---

## Demo Accounts

| Username | Password    | Role   |
|----------|-------------|--------|
| admin    | Admin123!   | Admin  |
| vendor1  | Vendor123!  | Vendor |
| vendor2  | Vendor123!  | Vendor |
| vendor3  | Vendor123!  | Vendor |

---

## Running Tests

### Backend tests (xUnit - 15 tests)

```bash
cd back/Savage.Tests
dotnet test --verbosity normal
```

### Frontend tests (Jest + React Testing Library - 9 tests)

```bash
cd front
CI=true npm test
```

### Run all tests at once

```bash
dotnet test back/Savage.Tests/Savage.Tests.csproj --verbosity normal && CI=true npm test --prefix front
```

---

## Features

### Authentication
- JWT bearer token authentication
- BCrypt password hashing
- Role-based authorization (Admin / Vendor)
- Auto-redirect on token expiry

### Frontend caching
- In-memory cache with configurable TTL (60s default)
- Cache invalidation on mutations (create/update/delete)
- Axios interceptors for automatic JWT attachment

### API integration
- RESTful API calls from React to ASP.NET Core backend
- CORS configured for local development
- Error handling with user-friendly alerts

### Design pattern
- Backend: Repository + Service layer with dependency injection
- Frontend: Container/Presenter with Context API for state management
- AutoMapper for model-to-DTO mapping

### UI design
- Cyberpunk/futuristic dark theme with neon cyan (#00f0ff) and magenta (#ff00ff) accents
- Glowing borders, gradient buttons, responsive sidebar navigation
- Google Fonts: Rajdhani + Orbitron

### Role-based experience

**Internal Admin can:**
- Sign in securely
- Create and manage inventory records
- Assign inventory to vendors and/or locations
- Manage vendor profiles
- Manage location records
- Update shipping status or shipment details
- View inventory with filtering (category, status) and sorting (date, title, quantity, SKU)
- Edit or remove any record

**External Vendor can:**
- Sign in securely
- View only inventory assigned to their vendor account
- View only shipments associated with their vendor
- See shipment/intake status
- Read-only dashboard with personal stats

---

## Database Model (MongoDB)

**Collections**: Users, InventoryItems, Vendors, Locations, Shipments

**InventoryItem fields**: id, sku, title, description, category, quantity, status, vendorId, intakeDate, locationId, shipmentStatus, notes

**Vendor fields**: id, name, contactEmail, contactPhone, address, status, createdAt

**Location fields**: id, name, address, type (Warehouse/Store/Distribution), capacity, currentOccupancy

**Shipment fields**: id, inventoryItemIds, vendorId, origin, destination, status, trackingNumber, shippedDate, estimatedArrival, notes

**User fields**: id, username, email, passwordHash, role, vendorId, createdAt

---

## MongoDB Setup and Manual Queries

### Connecting to the Mongo shell

Make sure MongoDB is running, then open the shell:

```bash
mongosh
```

Switch to the Savage database:

```
use SavageDb
```

### List all collections

```
show collections
```

Expected output: `InventoryItems`, `Locations`, `Shipments`, `Users`, `Vendors`

### Browse documents

View all documents in a collection (limit to 5 for readability):

```js
db.Vendors.find().limit(5).pretty()
db.InventoryItems.find().limit(5).pretty()
db.Locations.find().limit(5).pretty()
db.Shipments.find().limit(5).pretty()
db.Users.find({}, { passwordHash: 0 }).limit(5).pretty()
```

### Example queries

**Find all active vendors:**

```js
db.Vendors.find({ status: "Active" })
```

**Find a vendor by name:**

```js
db.Vendors.findOne({ name: "NeonTech Supplies" })
```

**Find all inventory items in the "Electronics" category:**

```js
db.InventoryItems.find({ category: "Electronics" })
```

**Find inventory items with quantity greater than 100:**

```js
db.InventoryItems.find({ quantity: { $gt: 100 } })
```

**Find inventory assigned to a specific vendor (by vendor ObjectId):**

```js
// First grab the vendor's _id
var v = db.Vendors.findOne({ name: "ByteWare Logistics" })
db.InventoryItems.find({ vendorId: v._id })
```

**Find items that are "Reserved" and flagged for review:**

```js
db.InventoryItems.find({ status: "Reserved", notes: "Flagged for review" })
```

**Count items per category:**

```js
db.InventoryItems.aggregate([
  { $group: { _id: "$category", count: { $sum: 1 } } },
  { $sort: { count: -1 } }
])
```

**Find all warehouse-type locations:**

```js
db.Locations.find({ type: "Warehouse" })
```

**Find locations where occupancy exceeds 50% capacity:**

```js
db.Locations.find({
  $expr: { $gt: ["$currentOccupancy", { $multiply: ["$capacity", 0.5] }] }
})
```

**Find all shipments currently in transit:**

```js
db.Shipments.find({ status: "InTransit" })
```

**Find shipments for a specific vendor with their tracking numbers:**

```js
var v = db.Vendors.findOne({ name: "NeonTech Supplies" })
db.Shipments.find(
  { vendorId: v._id },
  { trackingNumber: 1, status: 1, origin: 1, destination: 1 }
)
```

**Find all admin users:**

```js
db.Users.find({ role: "Admin" }, { passwordHash: 0 })
```

**Find which vendor a user belongs to:**

```js
var u = db.Users.findOne({ username: "vendor1" })
db.Vendors.findOne({ _id: u.vendorId })
```

### Useful maintenance commands

```js
// Count documents in each collection
db.InventoryItems.countDocuments()
db.Vendors.countDocuments()
db.Locations.countDocuments()
db.Shipments.countDocuments()
db.Users.countDocuments()

// Drop the entire database (use with caution -- reseeds on next backend start)
db.dropDatabase()
```

---

## API Endpoints

| Method | Endpoint              | Auth     | Description                     |
|--------|-----------------------|----------|---------------------------------|
| POST   | /api/auth/login       | Public   | Login and get JWT token         |
| POST   | /api/auth/register    | Public   | Register new user               |
| GET    | /api/inventory        | Any      | List inventory (vendor-scoped)  |
| GET    | /api/inventory/:id    | Any      | Get single item                 |
| POST   | /api/inventory        | Admin    | Create inventory item           |
| PUT    | /api/inventory/:id    | Admin    | Update inventory item           |
| DELETE | /api/inventory/:id    | Admin    | Delete inventory item           |
| GET    | /api/vendor           | Admin    | List all vendors                |
| POST   | /api/vendor           | Admin    | Create vendor                   |
| PUT    | /api/vendor/:id       | Admin    | Update vendor                   |
| DELETE | /api/vendor/:id       | Admin    | Delete vendor                   |
| GET    | /api/location         | Admin    | List all locations              |
| POST   | /api/location         | Admin    | Create location                 |
| PUT    | /api/location/:id     | Admin    | Update location                 |
| DELETE | /api/location/:id     | Admin    | Delete location                 |
| GET    | /api/shipment         | Any      | List shipments (vendor-scoped)  |
| POST   | /api/shipment         | Admin    | Create shipment                 |
| PUT    | /api/shipment/:id     | Admin    | Update shipment                 |
| DELETE | /api/shipment/:id     | Admin    | Delete shipment                 |

---

## AWS Deployment Notes

- **Compute**: Deploy backend and frontend on EC2 instances (or ECS)
- **Database**: Replace `mongodb://localhost:27017` with Amazon DocumentDB connection string in `appsettings.json`
- **Frontend**: Build with `npm run build` and serve via Nginx, S3+CloudFront, or EC2
- **Auth**: JWT is self-contained and works anywhere; can integrate AWS Cognito for production SSO
- **Environment**: Set `REACT_APP_API_URL` env var to point frontend at the production API URL

---

## Project Structure

```
savage/
  front/                    # React.js frontend
    src/
      components/           # Layout, ProtectedRoute
      contexts/             # AuthContext
      pages/                # Login, Dashboard
        admin/              # Inventory, Vendors, Locations, Shipments
      services/             # API service with caching
      theme/                # Cyberpunk MUI theme
  back/
    Savage.Api/             # ASP.NET Core 8 Web API
      Controllers/          # Auth, Inventory, Vendor, Location, Shipment
      Data/                 # DatabaseSeeder
      DTOs/                 # Request/Response DTOs
      Mapping/              # AutoMapper profiles
      Models/               # MongoDB document models
      Services/             # Business logic layer
    Savage.Tests/           # xUnit tests
  README.md
```
