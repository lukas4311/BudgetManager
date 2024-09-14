import { useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import { Carousel, CarouselContent, CarouselItem, CarouselNext, CarouselPrevious } from '../Shadcn/Carousel';
import BookmarkBorderOutlinedIcon from '@mui/icons-material/BookmarkBorderOutlined';
import React from 'react';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import { BankAccountApi } from '../../ApiClient/Main';
import _ from 'lodash';
import { BankAccountWithBalanceModel } from '../../Model/BankAccountWithBalanceModel';

class BankAccountSelectorProps {
    onBankAccountSelect?: (bankAccount: BankAccountWithBalanceModel) => void;
}

const BankAccountSelector = (props: BankAccountSelectorProps) => {
    const history = useHistory();
    const [bankAccounts, setBankAccounts] = React.useState<BankAccountWithBalanceModel[]>([]);
    const [selectedBankaccount, setSelectedBankaccount] = React.useState<number>(undefined);

    useEffect(() => {
        const apiFactory = new ApiClientFactory(history);
        const fetchData = async () => {
            const bankAccountApi = await apiFactory.getClient(BankAccountApi);
            const bankAccountsBalance = await bankAccountApi.bankAccountsAllBalanceToDateGet({ toDate: new Date(Date.now()) });
            const bankAccountInfo = await bankAccountApi.bankAccountsAllGet();
            setBankAccounts(bankAccountInfo.map(a => ({ bankAccountBalance: _.first(bankAccountsBalance.filter(b => b.id == a.id))?.balance, bankAccountName: a.code, bankAccountId: a.id })));
        }
        fetchData();
    }, [])

    const selectBankAccount = (bankAccountInfo: BankAccountWithBalanceModel) => {
        if (selectedBankaccount == bankAccountInfo.bankAccountId) {
            setSelectedBankaccount(undefined);
            props.onBankAccountSelect(undefined);
        }
        else {
            setSelectedBankaccount(bankAccountInfo.bankAccountId);
            props.onBankAccountSelect(bankAccountInfo);
        }
    }

    return (
        <Carousel>
            <CarouselContent>
                {bankAccounts.map(b =>
                (
                    <CarouselItem className="basis-1/2">
                        <div className='flex flex-col bg-battleshipGrey px-4 py-6 rounded-lg relative' onClick={_ => selectBankAccount(b)}>
                            {b.bankAccountId == selectedBankaccount ? <BookmarkBorderOutlinedIcon className='absolute top-4 right-4' /> : <></>}
                            <p className='text-3xl font-bold mb-2'>{b.bankAccountBalance},-</p>
                            <span className="ml-auto text-xl categoryIcon fill-white">{b.bankAccountName}</span>
                        </div>
                    </CarouselItem>
                ))}
            </CarouselContent>
            <CarouselPrevious />
            <CarouselNext />
        </Carousel>
    );
}

export { BankAccountSelectorProps, BankAccountSelector }