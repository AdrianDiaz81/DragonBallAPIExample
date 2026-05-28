import type { Character } from '../types/character';
import { CharacterCard } from './CharacterCard';

interface Props {
  characters: Character[];
}

export function CharacterGrid({ characters }: Props) {
  if (characters.length === 0) {
    return (
      <div className="col-span-full flex flex-col items-center justify-center py-24 text-gray-500">
        <span className="text-5xl mb-4">🐉</span>
        <p className="text-lg">No se encontraron personajes</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4">
      {characters.map((c) => (
        <CharacterCard key={c.id} character={c} />
      ))}
    </div>
  );
}
