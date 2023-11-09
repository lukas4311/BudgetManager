import React from 'react';

interface IRankingModel {
  score: number;
  title: string;
}

interface Props<T extends IRankingModel> {
  models: T[];
}

function ScoreList<T extends IRankingModel>({ models }: Props<T>) {
  const getHeight = (sequenceNo: number) => {
    const height = (models.length - (sequenceNo)) / models.length;
    return height * 100 + "%";
  }

  return (
    <div className="flex flex-row h-full justify-between">
      {models.map((model, index) => (
        <div className="flex flex-col h-full justify-end">
          <div key={index} style={{ height: getHeight(index) }}
            className="flex justify-center items-center duration-500 transition-all bg-white mx-4 lg:w-24 rounded-xl bg-vermilion mb-2 boxShadowHover">
            <div className="text-lg lg:text-2xl font-bold text-black">{model.score}</div>
          </div>
          <p>{model.title}</p>
        </div>
      ))}
    </div>
  );
}

export default ScoreList;