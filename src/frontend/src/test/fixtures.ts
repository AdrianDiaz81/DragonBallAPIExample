import type { Character } from '../types/character';

export const mockCharacter: Character = {
  id: 1,
  name: 'Goku',
  race: 'Saiyan',
  powerLevel: 9000,
  affiliation: 'Z Fighters',
  imageUrl: 'https://example.com/goku.webp',
};

export const mockCharacters: Character[] = [
  mockCharacter,
  { id: 2, name: 'Vegeta', race: 'Saiyan', powerLevel: 8500, affiliation: 'Z Fighters', imageUrl: 'https://example.com/vegeta.webp' },
  { id: 3, name: 'Frieza', race: 'Frieza Race', powerLevel: 120000, affiliation: 'Frieza Force', imageUrl: 'https://example.com/frieza.webp' },
];
