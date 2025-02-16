import React, { useState, useEffect } from 'react';
import { PaymentModel } from '../../ApiClient/Main/models';
import { IconsData } from '../../Enums/IconsEnum';

interface PaymentCategoryFilterProps {
    payments: PaymentModel[];
    onFilter: (selectedCategories: string[]) => void;
}

const PaymentCategoryFilter: React.FC<PaymentCategoryFilterProps> = ({ payments, onFilter }) => {
    const [categories, setCategories] = useState<{ code: string, icon: string }[]>([]);
    const [selectedCategories, setSelectedCategories] = useState<string[]>([]);
    const iconsData: IconsData = new IconsData();

    useEffect(() => {
        const uniqueCategories = Array.from(new Set(payments.map(payment => JSON.stringify({ code: payment.paymentCategoryCode, icon: payment.paymentCategoryIcon }))))
            .map(item => JSON.parse(item));
        setCategories(uniqueCategories);
    }, [payments]);

    const handleCategoryClick = (category: string) => {
        const updatedSelectedCategories = selectedCategories.includes(category) ? selectedCategories.filter(c => c !== category) : [...selectedCategories, category];
        setSelectedCategories(updatedSelectedCategories);
        onFilter(updatedSelectedCategories);
    };

    return (
        <div className="flex flex-col items-center">
            {categories.map(category => (
                <button
                    key={category.code}
                    onClick={() => handleCategoryClick(category.code)}
                    className={`p-2 mb-2 rounded-md categoryIcon w-12 ${selectedCategories.includes(category.code) ? 'bg-vermilion text-white' : 'bg-gray-200 text-gray-700'}`}
                >
                    {iconsData[category.icon]}
                </button>
            ))}
        </div>
    );
};

export default PaymentCategoryFilter;