CREATE TABLE hour
(
    id integer GENERATED ALWAYS as IDENTITY PRIMARY KEY NOT NULL,
    date date not null,
    start_at timestamptz not null,
    end_at timestamptz not null,
    updated_at timestamptz not null default CURRENT_TIMESTAMP,
    created_at timestamptz not null default CURRENT_TIMESTAMP
);