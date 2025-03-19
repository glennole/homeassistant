CREATE TABLE waterheater_usage
(
    id integer GENERATED ALWAYS as IDENTITY PRIMARY KEY NOT NULL,
    waterheater_id int references waterheater(id),
    hour_id int references hour(id),
    calculated_state bool not null default false,
    overriden_state bool null,
    consumption decimal null,
    cost decimal null,
    updated_at timestamptz not null default CURRENT_TIMESTAMP,
    created_at timestamptz not null default CURRENT_TIMESTAMP
);