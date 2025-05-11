from dataclasses import dataclass
import requests
from bs4 import BeautifulSoup


@dataclass
class FinData:
    year: str
    name: str
    value: str


@dataclass
class CompanyData:
    sector: str = ''
    industry: str = ''
    ceo: str = ''
    employees: str = ''
    city: str = ''


@dataclass
class FinSummary:
    pe: str
    fw_pe: str
    pe_to_sp: str
    div_yield: str
    market_cap: str


class RoicService:
    def get_main_financial_history(self, ticker: str):
        request = requests.get(f'https://roic.ai/financials/{ticker}')
        soup = BeautifulSoup(request.text, "html.parser")
        main = soup.findChild("main")
        children = main.findAll("div", recursive=False)[-1]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[0]
        children_base = children.findAll("div", recursive=False)[0]
        children = children_base.findAll("div", recursive=False)[-1]

        years = children_base.findAll("div", recursive=False)[-2]
        children_years = years.findAll("div", recursive=False)[0].findAll("div", recursive=False)[-1] \
            .findAll("div", recursive=False)

        year_data: list[str] = []

        for year in children_years:
            year_data.append(year.text)

        values = children.findAll("div", recursive=False)
        data: list[FinData] = []

        for value in values:
            text = value.findAll("span")[0]
            fin_data_base = value.findAll("div", recursive=False)[-1]
            all_find_data = fin_data_base.findAll("div", recursive=False)
            for i in range(0, len(year_data)):
                year = year_data[i]
                fin_data = all_find_data[i]

                if text.text == 'SEC Link':
                    continue

                model = FinData(year, text.text, fin_data.text)
                data.append(model)

        data = list(filter(lambda v: v.value != '' and v.value != '- -', data))
        return data

    def get_main_summary(self, ticker: str):
        request = requests.get(f'https://roic.ai/financials/{ticker}')
        soup = BeautifulSoup(request.text, "html.parser")
        main = soup.findChild("main")
        children = main.findAll("div", recursive=False)[1]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[-1]
        base = children.findAll("div", recursive=False)[-1]

        base_spans = base.findAll("span", recursive=True)
        pe = base_spans[0].text
        fwd_pe = base_spans[2].text
        pe_to_sp = base_spans[4].text
        market_cap = base_spans[6].text
        div_yield = base_spans[8].text

        fin_summary = FinSummary(pe, fwd_pe, pe_to_sp, div_yield, market_cap)
        return fin_summary

    def get_fin_summary(self, ticker: str):
        request = requests.get(f'https://roic.ai/company/{ticker}')
        soup = BeautifulSoup(request.text, "html.parser")
        main = soup.findChild("main")
        children = main.findAll("div", recursive=False)[-1]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[1:]
        fin_data_base_html = children[1:]
        years = children[0].findAll("div", recursive=False)[-1].findAll("div", recursive=False)
        year_data: list[str] = []

        for year in years:
            year_data.append(year.text)

        data: list[FinData] = []

        for value in fin_data_base_html:
            text = value.findAll("span")[0]
            fin_data_base = value.findAll("div", recursive=False)[-1]
            all_find_data = fin_data_base.findAll("div", recursive=False)
            for i in range(0, len(year_data)):
                year = year_data[i]
                fin_data = all_find_data[i]
                model = FinData(year, text.text, fin_data.text)
                data.append(model)

        data = list(filter(lambda v: v.value != '' and v.value != '- -', data))
        return data

    def get_main_company_data(self, ticker: str):
        request = requests.get(f'https://roic.ai/company/{ticker}')
        soup = BeautifulSoup(request.text, "html.parser")
        main = soup.findChild("main")
        children = main.findAll("div", recursive=False)[-1]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[-1]
        children = children.findAll("div", recursive=False)[-1]
        children = children.findAll("div", recursive=False)[0]
        children = children.findAll("div", recursive=False)[0]
        company_info = children.findAll("div", recursive=False)[-2]

        company_info = company_info.findAll("div", recursive=False)

        company = CompanyData()
        for companyInfoTag in company_info:
            inner_data = companyInfoTag.findAll("div", recursive=False)
            for data in inner_data:
                inner_desc = data.findAll("div", recursive=False)[0]
                inner_data = data.findAll("div", recursive=False)[1]
                desc = inner_desc.text[0:-2]
                company_data = inner_data.text

                if desc == "Sector":
                    company.sector = company_data
                elif desc == "Industry":
                    company.industry = company_data
                elif desc == "CEO":
                    company.ceo = company_data
                elif desc == "Full-time employees":
                    company.employees = company_data
                elif desc == "City":
                    company.city = company_data

        return company
