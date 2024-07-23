import base64 as b64
import csv
import io
import logging
from datetime import datetime
from typing import Dict
from Exceptions.ParseCsvError import ParseCsvError
from BrokerReportParser import BrokerReportParser
from ReportParsers.Trading212Parser import Trading212ReportParser
from Services.DB.StockRepository import StockRepository

log_name = 'Logs/trading212.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class ReportParser:
    __parserMap: Dict[str, BrokerReportParser] = {
        "Trading212": Trading212ReportParser()
    }

    def process_report_data(self, stock_repo: StockRepository):
        broker_report_data = stock_repo._get_all_stock_broker_reports_to_process()
        all_reports_data = []

        broker_parser_enum_type_id = stock_repo.get_enum_type('AvailableBrokerParsers')
        enums = stock_repo.get_enums_by_type_id(broker_parser_enum_type_id)

        for report_data in broker_report_data:
            try:
                broker = next((enum for enum in enums if enum.id == report_data.brokerId), None)
                parser = self.__parserMap[broker.code]
                self.parse_report_data_to_model(all_reports_data, parser, report_data)
            except Exception as e:
                print(e)
                stock_repo.changeProcessState(report_data.id, "ParsingError")

        for parsed_report in all_reports_data:
            try:
                stock_repo.store_trade_data(parsed_report["data"], "CZK", parsed_report["user_id"])
                stock_repo.changeProcessState(parsed_report["report_id"], "Finished")
            except Exception as e:
                logging.error(e)
                stock_repo.changeProcessState(parsed_report["report_id"], "SavingError")

    def parse_report_data_to_model(self, all_reports_data, parser: BrokerReportParser, report_data):
        try:
            parsed_csv = b64.b64decode(report_data.fileContentBase64).decode('utf-8')
            rows = csv.DictReader(io.StringIO(parsed_csv))
            records = []
            for row in rows:
                stock_record = parser.map_report_row_to_model(row)
                records.append(stock_record)

            all_reports_data.append(
                {"user_id": report_data.userIdentityId, "report_id": report_data.id, "data": records})
        except Exception as e:
            logging.error(e)
            raise ParseCsvError("Error while parsing CSV")


parser = ReportParser()
parser.process_report_data(StockRepository())
