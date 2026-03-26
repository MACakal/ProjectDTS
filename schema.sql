-- CREATE TABLE IF NOT EXISTS order_items;
-- CREATE TABLE IF NOT EXISTS orders;
-- CREATE TABLE IF NOT EXISTS products;
-- CREATE TABLE IF NOT EXISTS users;
CREATE TABLE IF NOT EXISTS users(
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
CREATE TABLE IF NOT EXISTS products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category VARCHAR(100),
    price NUMERIC(10, 2) NOT NULL CHECK (price > 0),
    rarity VARCHAR(50),
    view_count INT DEFAULT 0,
    purchase_count INT DEFAULT 0
);
ALTER TABLE products
ADD COLUMN IF NOT EXISTS name VARCHAR(255) NOT NULL;
ALTER TABLE products
ADD COLUMN IF NOT EXISTS description TEXT;
ALTER TABLE products
ADD COLUMN IF NOT EXISTS category VARCHAR(100);
ALTER TABLE products
ADD COLUMN IF NOT EXISTS price NUMERIC(10, 2) NOT NULL CHECK (price > 0);
ALTER TABLE products
ADD COLUMN IF NOT EXISTS rarity VARCHAR(50);
ALTER TABLE products
ADD COLUMN IF NOT EXISTS view_count INT DEFAULT 0;
ALTER TABLE products
ADD COLUMN IF NOT EXISTS purchase_count INT DEFAULT 0;
CREATE TABLE IF NOT EXISTS orders (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    purchased BOOLEAN DEFAULT FALSE,
    total_price NUMERIC(10, 2) DEFAULT 0.00
);
CREATE TABLE IF NOT EXISTS order_items (
    id SERIAL PRIMARY KEY,
    product_id INT NOT NULL REFERENCES products(id),
    order_id INT NOT NULL REFERENCES orders(id),
    quantity INT NOT NULL CHECK (quantity > 0)
);
INSERT INTO users (name, email, password, role)
VALUES (
        'admin',
        'admin',
        pgp_sym_encrypt('1234', 'admin_key'),
        'Admin'
    ) ON CONFLICT (email) DO NOTHING;
-- Fix voor de 'orders' tabel conflict
CREATE UNIQUE INDEX IF NOT EXISTS idx_one_active_basket_per_user ON orders (user_id)
WHERE (purchased = false);
ALTER TABLE order_items DROP CONSTRAINT IF EXISTS unique_order_product;
ALTER TABLE order_items
ADD CONSTRAINT unique_order_product UNIQUE (order_id, product_id);
CREATE OR REPLACE FUNCTION update_purchase_count() RETURNS TRIGGER AS $$ BEGIN IF NEW.purchased = true
    AND OLD.purchased = false THEN
UPDATE products p
SET purchase_count = p.purchase_count + oi.quantity
FROM order_items oi
WHERE oi.product_id = p.id
    AND oi.order_id = NEW.id;
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
DROP TRIGGER IF EXISTS trigger_update_purchase_count ON orders;
CREATE TRIGGER trigger_update_purchase_count
AFTER
UPDATE ON orders FOR EACH ROW EXECUTE FUNCTION update_purchase_count();
CREATE VIEW top3_cheapest AS
SELECT id,
    name,
    description,
    category,
    price,
    rarity
FROM products
ORDER BY price ASC
LIMIT 3;
CREATE VIEW top3_expensive AS
SELECT id,
    name,
    description,
    category,
    price,
    rarity
FROM products
ORDER BY price DESC
LIMIT 3;
CREATE VIEW most_popular_categories AS
SELECT category,
    SUM(purchase_count) AS total_purchases
FROM products
GROUP BY category
ORDER BY total_purchases DESC;
-- 1