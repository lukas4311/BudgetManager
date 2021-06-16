import datetime


def parse_arad_datetime_format(string_date: str):
    index = string_date.index("/")
    return datetime.datetime(int(string_date[index + 1:]), int(string_date[:index]), 1)

