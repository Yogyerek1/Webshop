IDENTITY (auth):
 - POST /auth/register (Guest)
 - POST /auth/login (Guest)
 - POST /auth/verify (Guest)
 - POST /auth/forgot-password (Guest)
 - POST /auth/reset-password (Guest)
 - POST /auth/request-update (User/Admin)
 - PUT /auth/update (User/Admin)
 - POST /auth/me (User/Admin)
 - GET /auth/logout (User/Admin)

PRODUCTS (Catalog):
 - GET /products (Guest/User/Admin)
 - GET /products/{id} (Guest/User/Admin)
 - POST /products (admin)
 - PUT /products/{id} (Admin)
 - DELETE /products/{id} (Admin)

CART (Shopping Cart):
 - GET /cart (User)
 - POST /cart/items (User)
 - DELETE /cart/items/{id} (user)

ORDERS & PAYMENTS (Checkout):
 - POST /orders/checkout (User)
 - GET /orders (Admin)