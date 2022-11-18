CREATE TABLE heavy_duty_switch
(
    id          integer GENERATED ALWAYS as IDENTITY PRIMARY KEY NOT NULL,
    heavy_duty_switch_id                                              varchar(255),
    reading_at  timestamptz                                      NOT NULL,
    state                                                        varchar(25),
    state_last_changed_at                                              timestamptz,
    accumulated_kwh                                              decimal,
    accumulated_kwh_last_changed_at             timestamptz,
    created_at   timestamptz                                      NOT NULL DEFAULT CURRENT_TIMESTAMP
);     