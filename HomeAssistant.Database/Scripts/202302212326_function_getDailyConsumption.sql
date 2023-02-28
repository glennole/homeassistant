CREATE OR REPLACE FUNCTION getDailyConsumption(timestamp without time zone)
    RETURNS TABLE (
        calculation_date date,
        consumption decimal(9,3),
        cost numeric
)
AS $$
    select date(first.reading_at) as calculation_date,  sum(second.accumulated_kwh - first.accumulated_kwh), sum((second.accumulated_kwh - first.accumulated_kwh) * daily_hour_price.price)
    from heavy_duty_switch as first
        join heavy_duty_switch as second on (date(second.reading_at) = (date(first.reading_at) + 1) AND extract(hour from first.reading_at) = 23 and extract(hour from second.reading_at) = 0)  or (date(first.reading_at) = date(second.reading_at) and extract(hour from second.reading_at) = extract(hour from first.reading_at) + 1)
    join daily_hour_price on date(first.reading_at) = daily_hour_price.date AND extract(hour from first.reading_at) = daily_hour_price.hour
        where  date(first.reading_at) >= $1
    group by date(first.reading_at)
    order by date(first.reading_at) asc
$$ LANGUAGE SQL;