import { Link } from 'react-router-dom';
import { AFFILIATION_COLORS, FALLBACK_IMAGE } from '../utils/affiliationColors';
import type { Character } from '../types/character';

interface Props {
  character: Character;
}

export function CharacterCard({ character }: Props) {
  const badgeColor = (character.affiliation && AFFILIATION_COLORS[character.affiliation]) ?? 'bg-gray-600';

  return (
    <Link
      to={`/characters/${character.id}`}
      className="group relative bg-[#111827] border border-[#1f2937] rounded-2xl overflow-hidden
                 transition-all duration-300 hover:border-orange-500 hover:shadow-[0_0_20px_rgba(249,115,22,0.3)]
                 hover:-translate-y-1"
    >
      <div className="relative h-56 bg-gradient-to-b from-[#1f2937] to-[#111827] flex items-end justify-center overflow-hidden">
        <img
          src={character.imageUrl}
          alt={character.name}
          className="h-full w-full object-contain object-bottom transition-transform duration-300 group-hover:scale-105"
          onError={(e) => {
            (e.target as HTMLImageElement).src = FALLBACK_IMAGE;
          }}
        />
        <div className="absolute inset-0 bg-gradient-to-t from-[#111827]/80 via-transparent to-transparent" />
      </div>

      <div className="p-4">
        <h2 className="text-lg font-bold text-white truncate group-hover:text-orange-400 transition-colors">
          {character.name} {character.lastName}
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
    </Link>
  );
}
