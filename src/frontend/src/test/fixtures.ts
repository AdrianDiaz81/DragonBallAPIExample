import type { Character } from '../types/character';

export const mockCharacter: Character = {
  id: 1,
  name: 'Goku',
  lastName: 'Son',
  race: 'Saiyan',
  powerLevel: 9000,
  description: 'El protagonista de la serie Dragon Ball Z.',
  affiliation: 'Z Fighters',
  imageUrl: 'https://example.com/goku.webp',
};

export const mockCharacters: Character[] = [
  mockCharacter,
  { id: 2, name: 'Vegeta', lastName: 'Ouji', race: 'Saiyan', powerLevel: 8500, affiliation: 'Z Fighters', imageUrl: 'https://example.com/vegeta.webp' },
  { id: 3, name: 'Frieza', lastName: 'Icejin', race: 'Frieza Race', powerLevel: 120000, affiliation: 'Frieza Force', imageUrl: 'https://example.com/frieza.webp' },
];
