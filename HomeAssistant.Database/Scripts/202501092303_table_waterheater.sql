CREATE TABLE waterheater
(
    id integer GENERATED ALWAYS as IDENTITY PRIMARY KEY NOT NULL,
    heavy_duty_switch_id varchar(255) null,
    name varchar(50) not null,
    updated_at timestamptz not null default CURRENT_TIMESTAMP,
    created_at timestamptz not null default CURRENT_TIMESTAMP
);