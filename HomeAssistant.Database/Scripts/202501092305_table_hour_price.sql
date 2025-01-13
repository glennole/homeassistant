CREATE TABLE hour_price
(
    id integer GENERATED ALWAYS as IDENTITY PRIMARY KEY NOT NULL,
    hour_id integer references hour(id),
    price decimal not null,
    updated_at timestamptz not null default CURRENT_TIMESTAMP,
    created_at timestamptz not null default CURRENT_TIMESTAMP
);