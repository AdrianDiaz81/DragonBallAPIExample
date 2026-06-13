export interface Character {
  id: number;
  name: string;
  lastName: string;
  race: string;
  powerLevel: number;
  description?: string;
  affiliation?: string;
  imageUrl?: string;
}
