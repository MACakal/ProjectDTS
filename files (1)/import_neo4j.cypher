// ============================================================
// Neo4j Import Script — ProjectDTS (Geüpdatet)
// Gebruik: plak per blok in Neo4j Browser
// ============================================================

// ── 0. Constraints (voorkomt dubbele data) ─────────────────
CREATE CONSTRAINT customer_id  IF NOT EXISTS FOR (c:Customer) REQUIRE c.customer_id IS UNIQUE;
CREATE CONSTRAINT product_id   IF NOT EXISTS FOR (p:Product)  REQUIRE p.product_id  IS UNIQUE;
CREATE CONSTRAINT order_id     IF NOT EXISTS FOR (o:Order)    REQUIRE o.order_id    IS UNIQUE;
CREATE CONSTRAINT category_name IF NOT EXISTS FOR (c:Category) REQUIRE c.name          IS UNIQUE;
CREATE CONSTRAINT review_id    IF NOT EXISTS FOR (r:Review)   REQUIRE r.review_id   IS UNIQUE;

// ── 1. Categories ──────────────────────────────────────────
// LET OP: We gebruiken nu 'name' als unieke identificatie in plaats van 'category_id'
LOAD CSV WITH HEADERS FROM 'file:///categories.csv' AS row
MERGE (c:Category {name: row.name})
SET c.brands = row.brands;

// ── 2. Customers ───────────────────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///customers.csv' AS row
MERGE (c:Customer {customer_id: toInteger(row.customer_id)})
SET c.first_name        = row.first_name,
    c.last_name         = row.last_name,
    c.email             = row.email,
    c.phone             = row.phone,
    c.street            = row.street,
    c.postal_code       = row.postal_code,
    c.loyalty_points    = toInteger(row.loyalty_points),
    c.registration_date = date(row.registration_date);

// ── 3 & 4. Products & Categorie Relaties ────────────────────
// Dit blok maakt de producten aan én legt direct de BELONGS_TO relatie
LOAD CSV WITH HEADERS FROM 'file:///products.csv' AS row
MERGE (p:Product {product_id: toInteger(row.Id)})
SET p.name           = row.Name,
    p.brand          = row.Brand,
    p.description    = row.Description,
    p.price          = toFloat(row.Price),
    p.rarity         = row.Rarity,
    p.view_count     = toInteger(row.View_count),
    p.purchase_count = toInteger(row.Purchase_count),
    p.stock          = toInteger(row.Stock),
    p.averageRating  = toFloat(row.AverageRating),
    p.ratingCount    = toInteger(row.RatingCount)

// Zoek of maak de categorie op basis van de tekstnaam uit de CSV
MERGE (c:Category {name: row.Category})
MERGE (p)-[:BELONGS_TO]->(c);

// ── 5 & 6. Orders & Customer Relaties ───────────────────────
LOAD CSV WITH HEADERS FROM 'file:///orders.csv' AS row
MERGE (o:Order {order_id: toInteger(row.order_id)})
SET o.order_date      = date(row.order_date),
    o.status          = row.status,
    o.total_amount    = toFloat(row.total_amount),
    o.payment_method  = row.payment_method,
    o.shipping_city   = row.shipping_city

WITH o, row
MATCH (c:Customer {customer_id: toInteger(row.customer_id)})
MERGE (c)-[:PLACED]->(o);

// ── 7. Order CONTAINS Product ──────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///order_items.csv' AS row
MATCH (o:Order   {order_id:   toInteger(row.order_id)})
MATCH (p:Product {product_id: toInteger(row.product_id)})
MERGE (o)-[r:CONTAINS {item_id: toInteger(row.item_id)}]->(p)
SET r.quantity   = toInteger(row.quantity),
    r.unit_price = toFloat(row.unit_price);

// ── 8, 9 & 10. Reviews + Relaties ──────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///reviews.csv' AS row
MERGE (r:Review {review_id: toInteger(row.review_id)})
SET r.rating     = toInteger(row.rating),
    r.comment    = row.comment,
    r.created_at = date(row.created_at)

WITH r, row
MATCH (c:Customer {customer_id: toInteger(row.customer_id)})
MATCH (p:Product  {product_id:  toInteger(row.product_id)})
MERGE (c)-[:REVIEWED]->(r)
MERGE (r)-[:ABOUT]->(p);

// ============================================================
// Verificatie
// ============================================================
// MATCH (n) RETURN labels(n), count(n) ORDER BY count(n) DESC;
// MATCH ()-[r]->() RETURN type(r), count(r) ORDER BY count(r) DESC;