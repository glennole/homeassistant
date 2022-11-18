CREATE TABLE daily_hour_price
(
    id          integer GENERATED ALWAYS as IDENTITY PRIMARY KEY NOT NULL,
    date        date                                             NOT NULL,
    hour        integer                                          NOT NULL,
    price       decimal                                          NOT NULL,
    description varchar(25) NULL,
    created_at   timestamptz                                      NOT NULL DEFAULT CURRENT_TIMESTAMP
);     