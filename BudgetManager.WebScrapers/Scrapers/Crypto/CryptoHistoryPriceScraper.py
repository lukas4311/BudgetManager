# importing datetime module
from datetime import datetime, timedelta
import time
import requests
import json
import logging

from Models.FilterTuple import FilterTuple
from Services.InfluxRepository import InfluxRepository
from enum import Enum
from configManager import token, organizaiton
from secret import influxDbUrl

log_name = 'Logs/cryptoPriceScraper.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)
influx_repository = InfluxRepository(influxDbUrl, "Crypto", token, organizaiton, logging)
measurement = "cryptoData"

class ResultData:
    def __init__(self, data):
        self.data = data


class Result:
    def __init__(self, timestamp, open_val, high_val, low_val, close_val, volume, quoteVolume):
        self.timestamp = timestamp
        self.open_val = open_val
        self.high_val = high_val
        self.low_val = low_val
        self.close_val = close_val
        self.volume = volume
        self.quoteVolume = quoteVolume


class CryptoTickers(Enum):
    BTC = "BTCUSD"
    ETH = "ETHUSD"


class CryptoWatchService:
    oneDayLimit = 86400
    cryptoWatchBaseUrl = "https://api.cryptowat.ch"

    def downloadCryptoPriceHistory(self, ticker: CryptoTickers):
        date_time = datetime(2000, 7, 26, 21, 20)
        lastRecordTime = self.get_last_record_time(ticker)
        print(lastRecordTime)
        now_datetime_with_offset = datetime.now().astimezone(lastRecordTime.tzinfo) - timedelta(days=1)
        print(lastRecordTime)

        if lastRecordTime < now_datetime_with_offset:
            fromTime = int(time.mktime(lastRecordTime.timetuple()))
            url = f"{self.cryptoWatchBaseUrl}/markets/coinbase-pro/{ticker.value}/ohlc?periods={self.oneDayLimit}&after={fromTime}"
            print(url)

            #response = requests.get(url)
            #jsonData = response.text
            jsonData = '{"result":{"86400":[[1686960000,25569.99,26486.56,25143.24,26329.6,16632.7425402,430829477.8089972894],[1687046400,26329.6,26783.2,26165.98,26506.56,7209.4251028,190902894.8021746814],[1687132800,26507.97,26693.11,26245.27,26336.46,5233.68378154,138724102.4576632209],[1687219200,26336.46,27040.64,26250,26837.78,9643.06748389,256406699.8593821993],[1687305600,26839.04,28417.11,26637.41,28320.43,28704.55295587,790630727.3632484315],[1687392000,28320.41,30800,28271.67,29995.08,31445.78143275,926713054.3703620523],[1687478400,29999.85,30513.25,29539.57,29886.31,18457.66785815,555153671.4476156661],[1687564800,29886.25,31443.67,29802.65,30707.61,22659.54058513,693410583.1208297001],[1687651200,30709.59,30815.92,30265.88,30547.18,6883.28318615,210671961.9674671174],[1687737600,30547.3,31057.86,30288.33,30480.24,6265.47434464,191964165.2029699373],[1687824000,30480.24,30662.66,29900,30272.18,12491.83081677,378324291.5182439978],[1687910400,30272.24,31020.54,30228.44,30697.38,11995.526853,367265335.3045582874],[1687996800,30696.06,30716.93,29840,30074.94,9391.66542094,284022599.5335527963],[1688083200,30074.93,30838,30036.18,30446.47,12261.34048625,374015421.2190067691],[1688169600,30445.67,31277,29417.14,30466.72,26013.22734909,791248579.6556080499],[1688256000,30466.73,30659.33,30312.8,30587.21,4012.76538882,122440425.502197421],[1688342400,30587.22,30791.75,30165.39,30613.51,4829.83871313,147476735.2263585665],[1688428800,30613.57,31399.08,30569,31161.8,11391.98257522,352524259.3921080091],[1688515200,31162.71,31333,30628.3,30771.25,7233.95188576,224200969.1312339407],[1688601600,30772.93,30882.95,30189.56,30499.27,8782.6192239,267751705.0327107737],[1688688000,30499.27,31525.1,29850,29899.38,17229.92231032,526558750.0455086581],[1688774400,29895.6,30456,29715.87,30352.08,11137.14221055,336449985.0367711078],[1688860800,30350.35,30388.91,30047.78,30291.69,3856.11078847,116537767.5301122848],[1688947200,30291.69,30451.78,30066.95,30168.38,3884.04687903,117576163.2890922397],[1689033600,30168.26,31055.75,29955,30419.89,12703.78162563,386142329.0777940458],[1689120000,30419.89,30811.65,30304.45,30626.55,10653.5761104,325174106.4179587093],[1689206400,30624.65,31000,30200,30381.81,14881.70511963,455339793.1647050038],[1689292800,30383.26,31862.21,30250.73,31471.83,26135.6192364,810832193.5563376489],[1689379200,31471.83,31645.66,29920.31,30330.52,19272.20339271,594376773.1520578573],[1689465600,30330.93,30406.27,30254.67,30301.65,2559.07232743,77592532.1813573152],[1689552000,30302.01,30457.03,30075.93,30247.64,4021.41012275,121849193.3081905082],[1689638400,30247.56,30345.45,29668.58,30143.08,10632.8066781,320220812.3564544213],[1689724800,30143.08,30249.3,29525,29861.93,11124.83936583,332525802.3871135794],[1689811200,29861.92,30196.24,29757.44,29916.72,8804.54675154,263898586.4634842174],[1689897600,29916.69,30421.29,29564.19,29809.13,11494.95348803,344234498.8361633229],[1689984000,29809.13,30060.28,29729.48,29907.98,7598.61053359,226979213.2443973446],[1690070400,29907.15,30002.38,29626.86,29795.03,3629.48251055,108379587.9950948071],[1690156800,29793.62,30350.7,29733.55,30081.61,4513.83214295,135461570.1254972344],[1690243200,30081.61,30099.99,28850,29176.98,14484.27557634,423903305.2719547224],[1690329600,29176.98,29337.65,29046,29222.31,4873.5282717,142208313.1819717511]]},"allowance":{"cost":0.015,"remaining":9.985,"upgrade":"For unlimited API access, create an account at https://cryptowat.ch"}}'
            print(jsonData)
            parsed_data = json.loads(jsonData)
            result_objects = [Result(*item) for item in parsed_data['result']['86400']]
            result_data_instance = ResultData(result_objects)
            stockPriceData = [d for d in result_data_instance.data if d.timestamp > fromTime]
            print(stockPriceData)
            for a in stockPriceData:
                print(f"Timestamp: {a.timestamp}, date: {datetime.fromtimestamp(a.timestamp)} Close Value: {a.close_val}")

    def get_last_record_time(self, ticker: CryptoTickers):
        lastValue = influx_repository.filter_last_value(measurement, FilterTuple("ticker", ticker.value), datetime.min)
        last_downloaded_time = datetime(2000, 1, 1)

        if len(lastValue) != 0:
            last_downloaded_time = lastValue[0].records[0]["_time"]

        return last_downloaded_time


cryptoService = CryptoWatchService();
cryptoService.downloadCryptoPriceHistory(CryptoTickers.BTC)