import { useNavigate } from 'react-router-dom';
import type { Character } from '../types/character';

interface Props {
  character: Character;
}

const AFFILIATION_COLORS: Record<string, string> = {
  'Z Fighters': 'bg-blue-600',
  'Frieza Force': 'bg-purple-600',
  'Red Ribbon Army': 'bg-red-600',
  'None': 'bg-gray-600',
};

export function CharacterCard({ character }: Props) {
  const navigate = useNavigate();
  const badgeColor = AFFILIATION_COLORS[character.affiliation] ?? 'bg-gray-600';

  return (
    <div
      onClick={() => navigate(`/characters/${character.id}`)}
      className="group relative bg-[#111827] border border-[#1f2937] rounded-2xl overflow-hidden cursor-pointer
                 transition-all duration-300 hover:border-orange-500 hover:shadow-[0_0_20px_rgba(249,115,22,0.3)]
                 hover:-translate-y-1"
    >
      <div className="relative h-56 bg-gradient-to-b from-[#1f2937] to-[#111827] flex items-end justify-center overflow-hidden">
        <img
          src={character.imageUrl}
          alt={character.name}
          className="h-full w-full object-contain object-bottom transition-transform duration-300 group-hover:scale-105"
          onError={(e) => {
            (e.target as HTMLImageElement).src =
              'https://dragonball-api.com/characters/goku_normal.webp';
          }}
        />
        <div className="absolute inset-0 bg-gradient-to-t from-[#111827]/80 via-transparent to-transparent" />
      </div>

      <div className="p-4">
        <h2 className="text-lg font-bold text-white truncate group-hover:text-orange-400 transition-colors">
          {character.name}
        </h2>
        <p className="text-sm text-gray-400 mt-1">{character.race}</p>

        <div className="flex items-center justify-between mt-3">
          <span className={`text-xs px-2 py-1 rounded-full text-white font-medium ${badgeColor}`}>
            {character.affiliation}
          </span>
          <span className="text-xs text-orange-400 font-mono font-semibold">
            ⚡ {character.powerLevel.toLocaleString()}
          </span>
        </div>
      </div>
    </div>
  );
}
