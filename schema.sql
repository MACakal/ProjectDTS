CREATE TABLE IF NOT EXISTS order_items;
CREATE TABLE IF NOT EXISTS orders;
CREATE TABLE IF NOT EXISTS products;
CREATE TABLE IF NOT EXISTS users;
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    address VARCHAR(255),
    zip_code VARCHAR(20),
    country VARCHAR(100),
    role VARCHAR(20) NOT NULL DEFAULT 'Customer',
    CHECK (role IN ('Customer', 'Admin'))
);
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category VARCHAR(100),
    price NUMERIC(10, 2) NOT NULL CHECK (price > 0),
    rarity VARCHAR(50)
);
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    purchased BOOLEAN DEFAULT FALSE,
    total_price NUMERIC(10, 2) DEFAULT 0.00
);
CREATE TABLE order_items (
    id SERIAL PRIMARY KEY,
    product_id INT NOT NULL REFERENCES products(id),
    order_id INT NOT NULL REFERENCES orders(id),
    quantity INT NOT NULL CHECK (quantity > 0)
);
INSERT INTO users (name, email, password, role)
VALUES ('admin', 'admin', '1234', 'Admin') ON CONFLICT (email) DO NOTHING;
-- Fix voor de 'orders' tabel conflict
CREATE UNIQUE INDEX IF NOT EXISTS idx_one_active_basket_per_user ON orders (user_id)
WHERE (purchased = false);
-- Fix voor de 'order_items' tabel conflict
ALTER TABLE order_items
ADD CONSTRAINT unique_order_product UNIQUE (order_id, product_id);