CREATE TABLE IF NOT EXISTS users(
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(10) NOT NULL DEFAULT 'customer'
);
CREATE TABLE IF NOT EXISTS products(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    details TEXT,
    price NUMERIC(10, 2) NOT NULL CHECK(price > 0),
    stock INT NOT NULL CHECK (stock >= 0)
);
INSERT INTO users (email, password_hash, role)
VALUES ('admin@webshop.com', 'admin12', 'admin') ON CONFLICT (email) DO NOTHING;