# 🏍️ eShop Backend

Este es el backend de una eShop construido con **.NET 8**, usando **Minimal APIs**, **Entity Framework Core**, **PostgreSQL**, **Redis** para manejar el carrito, y **Stripe** para pagos. El entorno de desarrollo está orquestado con **Docker**.

This is the backend of an eShop built with **.NET 8**, using **Minimal APIs**, **Entity Framework Core**, **PostgreSQL**, **Redis** for cart handling, and **Stripe** for payments. The development environment is orchestrated using **Docker**.

---

## 🧱 Tech Stack

- **.NET 8** (Minimal API)
- **Entity Framework Core**
- **PostgreSQL** (base de datos relacional / relational database)
- **Redis** (almacenamiento temporal para carrito / in-memory storage for cart)
- **Stripe** (procesamiento de pagos / payment processing)
- **Docker** (para servicios locales / for local services)

---

## 📁 Estructura del proyecto / Project Structure

```
eshopBackend/
│
├── docker-compose.yml
├── .env (to add)
│
└── Src/
    ├── Api/
    │   ├── .env.local (to add)
    │   └── ... (archivos del proyecto API / API project files)
    │
    └── Application/
        └── ... (lógica de aplicación / application logic)
```

---

## 🚀 Inicio rápido / Quick Start

### 1. Clonar el repositorio / Clone the repository

```bash
git clone https://github.com/LuisNivar/eshop-backend.git
cd eshop-backend
```

### 2. Configurar variables de entorno / Configure environment variables

- `.env`: contiene variables globales como claves de Stripe.
- `Src/Api/.env.local`: para variables locales específicas de la API.

Example `.env`:

```env
STRIPE_API_KEY=sk_test_xxxx
STRIPE_DEVICE_NAME=eshop-backend
```

Example `.env.local`:

```env
Stripe__ApiKey=sk_test_xxxx
Stripe__WebhookSecret=whsec_xxx
```

### 3. Levantar los servicios con Docker / Start services with Docker

> [!IMPORTANT]  
> Si utilizas docker-desktop puedes eliminar la siguiente linea / Remove the next line if using docker-desktop

> [!IMPORTANT]  
> Remplaza `192.168.x.x` por tu IP local / Replace `192.168.x.x` with your local IP

```yml
extra_hosts:
  - "host.docker.internal:192.168.x.x"
```

```bash
docker-compose up -d
```

Esto levantará / This will start:

- PostgreSQL (port 5432)
- Redis (port 6379)
- Stripe CLI (listening for events)

Stripe CLI te arrojará un log / will output:

```
Ready! You are using Stripe API Version ... Your webhook signing secret is whsec_xxx...
```

Usa ese valor para `Stripe__WebhookSecret` en `.env.local` / Use that secret in `.env.local`

### 4. Ejecutar la API / Run the API

En una terminal separada / In a separate terminal:

```bash
cd Src/Api
dotnet ef database update --project ../Application
dotnet run
```

> Asegúrate que el puerto por defecto (`5256`) esté libre / Make sure port `5256` is free.

---

## 🌐 Endpoints principales / Main Endpoints

### 📦 CatalogEndpoint

- `GET /products` — Obtener todos los productos / Get all products
- `GET /products/{id}` — Obtener un producto por ID / Get product by ID
- `POST /products` — Crear un nuevo producto / Create a new product
- `DELETE /products/{id}` — Eliminar un producto / Delete a product

### 🛒 CartEndpoint

- `GET /carts/{userId}` — Obtener carrito por usuario / Get cart by user
- `POST /cart/item` — Agregar item al carrito / Add item to cart
- `PUT /cart/item` — Actualizar item del carrito / Update item in cart
- `DELETE /cart/{userId}` — Vaciar carrito del usuario / Empty cart
- `DELETE /cart/item` — Eliminar item del carrito / Remove item from cart

### 🚐 OrderEndpoint

- `GET /order` — Obtener todas las órdenes / Get all orders
- `GET /order/{id}` — Obtener orden por ID / Get order by ID
- `GET /order/{userId}` — Obtener órdenes por usuario / Get orders by user

### 💳 PaymentEndpoint

- `POST /pay/{orderId}` — Iniciar pago / Start payment
- `POST /webhook` — Webhook de Stripe / Stripe webhook

---

## 📫 Webhooks con Stripe CLI

El contenedor `stripe-cli` escucha eventos y los reenvía a tu API / listens and forwards events:

```bash
stripe listen --forward-to http://host.docker.internal:5256/webhook
```

Verifica que tu endpoint `/webhook` esté implementado / Make sure `/webhook` is implemented.

---

## 🐘 Acceso a la base de datos / Database Access

- **Host:** `localhost`
- **Puerto / Port:** `5432`
- **Base de datos / DB:** `db_development`
- **Usuario / User:** `postgres`
- **Contraseña / Password:** `development`

---

## 🩰 Migraciones EF Core / EF Core Migrations

Agregar migración / Add migration:

```bash
cd Src/Api
dotnet ef migrations add InitialCreate --project ../Application
dotnet ef database update
```

> Asegurate de tener instalado el CLI de EF Core / Make sure EF CLI is installed:
> `dotnet tool install --global dotnet-ef`

---

## Licencia / License

Este proyecto está licenciado bajo la licencia MIT. / This project is licensed under the MIT license.
Consulta el archivo [LICENSE](LICENSE) para más detalles. / See [LICENSE](LICENSE) for more details.
