import datetime

def parse_arad_datetime_format(string_date: str) -> datetime:
    """
    Parses a date string in the format 'MM/YYYY' and returns a datetime object
    representing the first day of that month.

    Args:
        string_date (str): A string representing a date in 'MM/YYYY' format.

    Returns:
        datetime: A datetime object set to the first day of the given month and year.
    """
    index = string_date.index("/")
    return datetime.datetime(int(string_date[index + 1:]), int(string_date[:index]), 1)

