import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { mockCharacters } from '../test/fixtures';
import { CharacterGrid } from './CharacterGrid';

const renderGrid = (characters = mockCharacters) =>
  render(
    <MemoryRouter>
      <CharacterGrid characters={characters} />
    </MemoryRouter>,
  );

describe('CharacterGrid', () => {
  it('renders one card per character', () => {
    renderGrid();
    expect(screen.getAllByRole('img')).toHaveLength(mockCharacters.length);
  });

  it('renders all character names', () => {
    renderGrid();
    expect(screen.getByText('Goku')).toBeInTheDocument();
    expect(screen.getByText('Vegeta')).toBeInTheDocument();
    expect(screen.getByText('Frieza')).toBeInTheDocument();
  });

  it('shows empty state message when list is empty', () => {
    renderGrid([]);
    expect(screen.getByText('No se encontraron personajes')).toBeInTheDocument();
  });

  it('does not show empty state when there are characters', () => {
    renderGrid();
    expect(screen.queryByText('No se encontraron personajes')).not.toBeInTheDocument();
  });
});
