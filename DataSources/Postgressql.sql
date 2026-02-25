DROP TABLE IF EXISTS ProductItem CASCADE;
DROP TABLE IF EXISTS "order" CASCADE;
DROP TABLE IF EXISTS Customer CASCADE;
DROP TABLE IF EXISTS Product CASCADE;
DROP TABLE IF EXISTS "User" CASCADE;

CREATE TABLE "User" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    address VARCHAR(255),
    zip_code VARCHAR(20),
    country VARCHAR(100),
    email VARCHAR(255) UNIQUE NOT NULL
);

CREATE TABLE Product (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category VARCHAR(100),
    price DECIMAL(10, 2) NOT NULL,
    rarity VARCHAR(50)
);

CREATE TABLE Customer (
    id SERIAL PRIMARY KEY,
    user_id INT REFERENCES "User"(id) ON DELETE SET NULL,
    name VARCHAR(255) NOT NULL,
    address VARCHAR(255),
    zip_code VARCHAR(20),
    country VARCHAR(100),
    email VARCHAR(255)
);

CREATE TABLE "order" (
    id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES Customer(id),
    purchased BOOLEAN DEFAULT FALSE,
    total_price DECIMAL(10, 2) DEFAULT 0.00
);

CREATE TABLE ProductItem (
    id SERIAL PRIMARY KEY,
    product_id INT NOT NULL REFERENCES Product(id),
    order_id INT NOT NULL REFERENCES "order"(id),
    quantity INT NOT NULL CHECK (quantity > 0)
);
