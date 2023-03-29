import React from 'react';

interface IRankingModel {
  score: number;
}

interface Props<T extends IRankingModel> {
  models: T[];
}

function ScoreList<T extends IRankingModel>({ models }: Props<T>) {
  const maxScore = Math.max(...models.map(model => model.score));
  const heights = models.map(model => (model.score / maxScore) * 100 + '%');

  return (
    <div className="flex flex-col space-y-5">
      {models.map((model, index) => (
        <div
          key={index}
          className="flex justify-center items-center bg-lightgray shadow-md h-[100px] sm:h-[150px] md:h-[200px] lg:h-[250px] xl:h-[300px] hover:shadow-lg transition-all duration-300"
          style={{ height: heights[index] }}
        >
          <div className="text-3xl font-bold text-black">{model.score}</div>
        </div>
      ))}
    </div>
  );
}

export default ScoreList;