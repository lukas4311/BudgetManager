import base64 as b64
import csv
import io
import logging
from datetime import datetime
from Exceptions.ParseCsvError import ParseCsvError
from ReportParsers import BrokerReportParser
from Services.DB.StockRepository import StockRepository

log_name = 'Logs/trading212.' + datetime.now().strftime('%Y-%m-%d') + '.log'
logging.basicConfig(filename=log_name, filemode='a', format='%(name)s - %(levelname)s - %(message)s',
                    level=logging.DEBUG)


class ReportParser:
    def process_report_data(self, stock_repo: StockRepository, parser: BrokerReportParser):
        broker_report_data = stock_repo._get_all_stock_broker_reports_to_process()
        all_reports_data = []

        for report_data in broker_report_data:
            try:
                self.parse_report_data_to_model(all_reports_data, parser, report_data)
            except Exception as e:
                stock_repo.changeProcessState(report_data.id, "ParsingError")

        print(all_reports_data)
        for parsed_report in all_reports_data:
            try:
                stock_repo.store_trade_data(parsed_report["data"], "CZK", parsed_report["user_id"])
                stock_repo.changeProcessState(parsed_report["report_id"], "Finished")
            except Exception as e:
                print(parsed_report)
                stock_repo.changeProcessState(parsed_report["report_id"], "SavingError")

    def parse_report_data_to_model(all_reports_data, parser: BrokerReportParser, report_data):
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
            raise ParseCsvError("Error while parsing CSV")
