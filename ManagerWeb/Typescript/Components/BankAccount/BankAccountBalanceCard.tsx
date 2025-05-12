import { useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import React from 'react';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import { BankAccountApi } from '../../ApiClient/Main';
import _ from 'lodash';
import { BankAccountWithBalanceModel } from '../../Model/BankAccountWithBalanceModel';

class BankAccountBalanceCardProps {
    onBankAccountSelect?: (bankAccount: BankAccountWithBalanceModel) => void;
    cardClass?: string;
}

const BankAccountBalanceCard = (props: BankAccountBalanceCardProps) => {
    const history = useHistory();
    const [total, setTotal] = React.useState<number>(0);

    useEffect(() => {
        const apiFactory = new ApiClientFactory(history);
        const fetchData = async () => {
            const bankAccountApi = await apiFactory.getClient(BankAccountApi);
            const bankAccountsBalance = await bankAccountApi.v1BankAccountsAllBalanceToDateGet({ toDate: new Date(Date.now()) });
            const total = _.sumBy(bankAccountsBalance, s => s.balance);
            setTotal(total);
        }
        fetchData();
    }, [])

    return (
        <div className={`flex flex-col bg-battleshipGrey px-4 py-6 rounded-lg relative ${props?.cardClass ?? ""}`}>
            <span className="text-2xl text-left font-semibold categoryIcon fill-white">Current balance</span>
            <p className='text-4xl text-center font-black mb-2'>{total}</p>
        </div>
    );
}

export { BankAccountBalanceCardProps, BankAccountBalanceCard }