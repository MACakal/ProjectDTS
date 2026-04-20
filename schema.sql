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
CREATE UNIQUE INDEX IF NOT EXISTS idx_unique_product_name ON products(name);
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
ALTER TABLE order_items DROP CONSTRAINT IF EXISTS unique_order_product;
ALTER TABLE order_items
ADD CONSTRAINT unique_order_product UNIQUE (order_id, product_id);
CREATE TABLE IF NOT EXISTS ratings (
    id SERIAL PRIMARY KEY,
    product_id INT NOT NULL REFERENCES products(id) ON DELETE CASCADE,
    user_id INT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    rating_value INT NOT NULL CHECK (rating_value >= 1 AND rating_value <= 5),
    review_text TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_user_product_rating UNIQUE (product_id, user_id)
);
CREATE INDEX IF NOT EXISTS idx_ratings_product_id ON ratings(product_id);
CREATE INDEX IF NOT EXISTS idx_ratings_user_id ON ratings(user_id);
-- CREATE EXTENSION IF NOT EXISTS pgcrypto;
INSERT INTO users (name, email, password, role)
VALUES (
        'admin',
        'admin',
        '$2a$11$JTuZ1sESk70FH2gwDmUIEepFu743L8bMydf2uTeiqIdIHzgPhEgnK',
        'Admin'
    ) ON CONFLICT (email) DO NOTHING;
-- INSERT INTO users (name, email, password, role)
-- VALUES (
--         'admin',
--         'admin',
--         pgp_sym_encrypt('1234', 'admin_key'),
--         'Admin'
--     ) ON CONFLICT (email) DO NOTHING;
CREATE UNIQUE INDEX IF NOT EXISTS idx_one_active_basket_per_user ON orders (user_id)
WHERE purchased = false;
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
INSERT INTO products (name, description, category, price, rarity)
VALUES (
        'Basic T-Shirt',
        'Plain cotton t-shirt',
        'Clothing',
        9.99,
        'Common'
    ),
    (
        'Leather Jacket',
        'Stylish leather jacket',
        'Clothing',
        199.99,
        'Rare'
    ),
    (
        'Jeans Classic',
        'Denim jeans',
        'Clothing',
        49.99,
        'Common'
    ),
    (
        'Hoodie Comfort',
        'Warm hoodie',
        'Clothing',
        59.99,
        'Uncommon'
    ),
    (
        'Sneakers Runner',
        'Running shoes',
        'Clothing',
        89.99,
        'Uncommon'
    ),
    (
        'Gaming Mouse',
        'RGB gaming mouse',
        'Electronics',
        49.99,
        'Uncommon'
    ),
    (
        'Mechanical Keyboard',
        'Mechanical keyboard with RGB',
        'Electronics',
        89.99,
        'Uncommon'
    ),
    (
        'Smartphone X',
        'Latest smartphone model',
        'Electronics',
        999.99,
        'Epic'
    ),
    (
        'Wireless Earbuds',
        'Noise-cancelling earbuds',
        'Electronics',
        149.99,
        'Rare'
    ),
    (
        '4K Monitor',
        'High-resolution monitor',
        'Electronics',
        399.99,
        'Epic'
    ),
    (
        'Laptop Pro',
        'High-end laptop',
        'Electronics',
        1999.99,
        'Legendary'
    ),
    (
        'VR Headset',
        'Virtual reality headset',
        'Electronics',
        399.99,
        'Epic'
    ),
    (
        'Smartwatch',
        'Fitness tracking smartwatch',
        'Electronics',
        199.99,
        'Rare'
    ),
    (
        'Novel: The Adventure',
        'A thrilling adventure book',
        'Books',
        14.99,
        'Common'
    ),
    (
        'Cookbook Deluxe',
        'Gourmet recipes',
        'Books',
        29.99,
        'Uncommon'
    ),
    (
        'Science Encyclopedia',
        'Educational science book',
        'Books',
        49.99,
        'Rare'
    ),
    (
        'Mystery Thriller',
        'Suspenseful mystery novel',
        'Books',
        19.99,
        'Common'
    ),
    (
        'Yoga Mat',
        'Non-slip yoga mat',
        'Sports',
        24.99,
        'Common'
    ),
    (
        'Dumbbell Set',
        'Adjustable dumbbells 20kg',
        'Sports',
        89.99,
        'Uncommon'
    ),
    (
        'Treadmill',
        'Home treadmill',
        'Sports',
        799.99,
        'Rare'
    ),
    (
        'Mountain Bike',
        'Full suspension mountain bike',
        'Sports',
        1200.00,
        'Epic'
    ),
    (
        'Basketball',
        'Official size basketball',
        'Sports',
        29.99,
        'Common'
    ),
    (
        'Collector Figurine',
        'Limited edition figurine',
        'Collectibles',
        299.99,
        'Legendary'
    ),
    (
        'Poster: Space Art',
        'High-quality poster print',
        'Collectibles',
        19.99,
        'Common'
    ),
    (
        'Board Game: Strategy',
        'Fun strategy board game',
        'Games',
        49.99,
        'Uncommon'
    ),
    (
        'Puzzle 1000 pcs',
        'Challenging puzzle',
        'Games',
        19.99,
        'Common'
    ),
    (
        'VR Board Game',
        'Mixed reality board game',
        'Games',
        149.99,
        'Rare'
    ),
    (
        'Action Figure',
        'Superhero action figure',
        'Collectibles',
        39.99,
        'Uncommon'
    ),
    (
        'Plush Toy',
        'Soft plush toy',
        'Toys',
        14.99,
        'Common'
    ),
    (
        'RC Car',
        'Remote-controlled car',
        'Toys',
        49.99,
        'Uncommon'
    ),
    (
        'Drone Mini',
        'Beginner drone',
        'Electronics',
        199.99,
        'Rare'
    ),
    (
        'Headphones Studio',
        'Studio-grade headphones',
        'Electronics',
        249.99,
        'Epic'
    ),
    (
        'Camping Tent',
        '2-person camping tent',
        'Sports',
        129.99,
        'Uncommon'
    ),
    (
        'Hiking Backpack',
        'Durable backpack',
        'Sports',
        89.99,
        'Common'
    ),
    (
        'Fitness Tracker',
        'Wearable fitness tracker',
        'Electronics',
        99.99,
        'Uncommon'
    ),
    (
        'Smart Lamp',
        'Voice-controlled lamp',
        'Electronics',
        49.99,
        'Common'
    ),
    (
        'Desk Chair',
        'Ergonomic office chair',
        'Furniture',
        149.99,
        'Uncommon'
    ),
    (
        'Coffee Table',
        'Wooden coffee table',
        'Furniture',
        99.99,
        'Common'
    ),
    (
        'Gaming Chair',
        'High-end gaming chair',
        'Furniture',
        299.99,
        'Rare'
    ),
    (
        'Wall Art Painting',
        'Canvas wall painting',
        'Decor',
        79.99,
        'Uncommon'
    ),
    (
        'LED Light Strip',
        'RGB LED strip',
        'Decor',
        39.99,
        'Common'
    ) ON CONFLICT (name) DO NOTHING;
CREATE OR REPLACE VIEW top3_cheapest AS
SELECT id,
    name,
    description,
    category,
    price,
    rarity
FROM products
ORDER BY price ASC
LIMIT 3;
CREATE OR REPLACE VIEW top3_expensive AS
SELECT id,
    name,
    description,
    category,
    price,
    rarity
FROM products
ORDER BY price DESC
LIMIT 3;
CREATE OR REPLACE VIEW most_popular_categories AS
SELECT category,
    SUM(purchase_count) AS total_purchases
FROM products
GROUP BY category
ORDER BY total_purchases DESC;
CREATE OR REPLACE VIEW user_spending AS
SELECT u.id,
    u.name,
    o.total_price,
    o.created_at
FROM users u
    LEFT JOIN orders o ON u.id = o.user_id
    AND o.purchased = true
WHERE u.role != 'Admin';
ALTER TABLE products
ADD COLUMN IF NOT EXISTS stock INT DEFAULT 10 CHECK (stock >= 0);
CREATE TABLE IF NOT EXISTS admin_notifications (
    id SERIAL PRIMARY KEY,
    product_id INT REFERENCES products(id),
    message TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_read BOOLEAN DEFAULT FALSE
);
CREATE OR REPLACE FUNCTION check_low_stock() RETURNS TRIGGER AS $$ BEGIN IF NEW.stock < 5 THEN
INSERT INTO admin_notifications (product_id, message)
VALUES (
        NEW.id,
        'The stock of "' || NEW.name || '" is low. There are only ' || NEW.stock || ' left.'
    );
END IF;
RETURN NEW;
END;
$$ LANGUAGE plpgsql;
DROP TRIGGER IF EXISTS trigger_check_low_stock ON products;
CREATE TRIGGER trigger_check_low_stock
AFTER
UPDATE OF stock ON products FOR EACH ROW EXECUTE FUNCTION check_low_stock();