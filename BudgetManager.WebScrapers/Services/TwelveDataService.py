class TwelveDataService:
    logging: object
    apiToken: str
    
    def __init__(self, apiToken: str, logging:object):
        self.logging = logging
        self.token = apiToken
