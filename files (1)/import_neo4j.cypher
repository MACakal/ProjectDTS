// ============================================================
// Neo4j Import Script — ProjectDTS
// Gebruik: plak per blok in Neo4j Browser
// Of run via: cypher-shell -f import_neo4j.cypher
//
// Zet de CSV bestanden in de Neo4j import map:
//   Windows: C:\Users\<naam>\.Neo4jDesktop\relate-data\dbmss\<db-id>\import\
//   Mac/Linux: ~/.neo4j/import/   of   /var/lib/neo4j/import/
// ============================================================

// ── 0. Constraints (voorkomt dubbele data) ─────────────────
CREATE CONSTRAINT customer_id  IF NOT EXISTS FOR (c:Customer)  REQUIRE c.customer_id  IS UNIQUE;
CREATE CONSTRAINT product_id   IF NOT EXISTS FOR (p:Product)   REQUIRE p.product_id   IS UNIQUE;
CREATE CONSTRAINT order_id     IF NOT EXISTS FOR (o:Order)     REQUIRE o.order_id     IS UNIQUE;
CREATE CONSTRAINT category_id  IF NOT EXISTS FOR (c:Category)  REQUIRE c.category_id  IS UNIQUE;
CREATE CONSTRAINT review_id    IF NOT EXISTS FOR (r:Review)    REQUIRE r.review_id    IS UNIQUE;

// ── 1. Categories ──────────────────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///categories.csv' AS row
MERGE (c:Category {category_id: toInteger(row.category_id)})
SET c.name   = row.name,
    c.brands = row.brands;

// ── 2. Customers ───────────────────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///customers.csv' AS row
MERGE (c:Customer {customer_id: toInteger(row.customer_id)})
SET c.first_name         = row.first_name,
    c.last_name          = row.last_name,
    c.email              = row.email,
    c.phone              = row.phone,
    c.street             = row.street,
    c.postal_code        = row.postal_code,
    c.loyalty_points     = toInteger(row.loyalty_points),
    c.registration_date  = date(row.registration_date);

// ── 3. Products ────────────────────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///products.csv' AS row
MERGE (p:Product {product_id: toInteger(row.product_id)})
SET p.name       = row.name,
    p.brand      = row.brand,
    p.price      = toFloat(row.price),
    p.unit_price = toFloat(row.unit_price),
    p.stock      = toInteger(row.stock),
    p.weight_kg  = toFloat(row.weight_kg);

// ── 4. Product BELONGS_TO Category ────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///products.csv' AS row
MATCH (p:Product  {product_id:  toInteger(row.product_id)})
MATCH (c:Category {category_id: toInteger(row.category_id)})
MERGE (p)-[:BELONGS_TO]->(c);

// ── 5. Orders ──────────────────────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///orders.csv' AS row
MERGE (o:Order {order_id: toInteger(row.order_id)})
SET o.order_date      = date(row.order_date),
    o.status          = row.status,
    o.total_amount    = toFloat(row.total_amount),
    o.payment_method  = row.payment_method,
    o.shipping_city   = row.shipping_city;

// ── 6. Customer PLACED Order ───────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///orders.csv' AS row
MATCH (c:Customer {customer_id: toInteger(row.customer_id)})
MATCH (o:Order    {order_id:    toInteger(row.order_id)})
MERGE (c)-[:PLACED]->(o);

// ── 7. Order CONTAINS Product ──────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///order_items.csv' AS row
MATCH (o:Order   {order_id:   toInteger(row.order_id)})
MATCH (p:Product {product_id: toInteger(row.product_id)})
MERGE (o)-[r:CONTAINS {item_id: toInteger(row.item_id)}]->(p)
SET r.quantity   = toInteger(row.quantity),
    r.unit_price = toFloat(row.unit_price);

// ── 8. Reviews ─────────────────────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///reviews.csv' AS row
MERGE (r:Review {review_id: toInteger(row.review_id)})
SET r.rating     = toInteger(row.rating),
    r.comment    = row.comment,
    r.created_at = date(row.created_at);

// ── 9. Customer REVIEWED Review ────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///reviews.csv' AS row
MATCH (c:Customer {customer_id: toInteger(row.customer_id)})
MATCH (r:Review   {review_id:   toInteger(row.review_id)})
MERGE (c)-[:REVIEWED]->(r);

// ── 10. Review ABOUT Product ───────────────────────────────
LOAD CSV WITH HEADERS FROM 'file:///reviews.csv' AS row
MATCH (r:Review  {review_id:  toInteger(row.review_id)})
MATCH (p:Product {product_id: toInteger(row.product_id)})
MERGE (r)-[:ABOUT]->(p);

// ============================================================
// Verificatie — run dit na de import om te controleren
// ============================================================
// MATCH (n) RETURN labels(n), count(n) ORDER BY count(n) DESC;
// MATCH ()-[r]->() RETURN type(r), count(r) ORDER BY count(r) DESC;
