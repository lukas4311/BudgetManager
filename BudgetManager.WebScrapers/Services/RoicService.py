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
        childrenBase = children.findAll("div", recursive=False)[0]
        children = childrenBase.findAll("div", recursive=False)[-1]

        years = childrenBase.findAll("div", recursive=False)[-2]
        childrenYears = years.findAll("div", recursive=False)[0].findAll("div", recursive=False)[-1] \
            .findAll("div", recursive=False)

        yearData: list[str] = []

        for year in childrenYears:
            yearData.append(year.text)

        values = children.findAll("div", recursive=False)
        data: list[FinData] = []

        for value in values:
            text = value.findAll("span")[0]
            finDataBase = value.findAll("div", recursive=False)[-1]
            allFindData = finDataBase.findAll("div", recursive=False)
            for i in range(0, len(yearData)):
                year = yearData[i]
                fin_data = allFindData[i]
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

        baseSpans = base.findAll("span", recursive=True)
        pe = baseSpans[0].text
        fwd_pe = baseSpans[2].text
        pe_to_sp = baseSpans[4].text
        market_cap = baseSpans[6].text
        div_yield = baseSpans[8].text

        finSummary = FinSummary(pe, fwd_pe, pe_to_sp, div_yield, market_cap)
        return finSummary

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
        yearData: list[str] = []

        for year in years:
            yearData.append(year.text)

        data: list[FinData] = []

        for value in fin_data_base_html:
            text = value.findAll("span")[0]
            finDataBase = value.findAll("div", recursive=False)[-1]
            allFindData = finDataBase.findAll("div", recursive=False)
            for i in range(0, len(yearData)):
                year = yearData[i]
                fin_data = allFindData[i]
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
        companyInfo = children.findAll("div", recursive=False)[-2]

        companyInfo = companyInfo.findAll("div", recursive=False)

        company = CompanyData()
        for companyInfoTag in companyInfo:
            innerData = companyInfoTag.findAll("div", recursive=False)
            for data in innerData:
                innerDesc = data.findAll("div", recursive=False)[0]
                innerData = data.findAll("div", recursive=False)[1]
                desc = innerDesc.text[0:-2]
                companyData = innerData.text

                if desc == "Sector":
                    company.sector = companyData
                elif desc == "Industry":
                    company.industry = companyData
                elif desc == "CEO":
                    company.ceo = companyData
                elif desc == "Full-time employees":
                    company.employees = companyData
                elif desc == "City":
                    company.city = companyData

        return company
