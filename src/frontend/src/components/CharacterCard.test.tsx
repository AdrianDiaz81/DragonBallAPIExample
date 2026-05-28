import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { mockCharacter } from '../test/fixtures';
import { CharacterCard } from './CharacterCard';

const renderCard = (overrides = {}) =>
  render(
    <MemoryRouter>
      <CharacterCard character={{ ...mockCharacter, ...overrides }} />
    </MemoryRouter>,
  );

describe('CharacterCard', () => {
  it('renders the character name', () => {
    renderCard();
    expect(screen.getByText('Goku')).toBeInTheDocument();
  });

  it('renders the character race', () => {
    renderCard();
    expect(screen.getByText('Saiyan')).toBeInTheDocument();
  });

  it('renders the affiliation badge', () => {
    renderCard();
    expect(screen.getByText('Z Fighters')).toBeInTheDocument();
  });

  it('renders the power level formatted', () => {
    renderCard();
    expect(screen.getByText(/9[,.]000/)).toBeInTheDocument();
  });

  it('renders the character image with correct alt text', () => {
    renderCard();
    expect(screen.getByRole('img', { name: 'Goku' })).toBeInTheDocument();
  });

  it('navigates to character detail on click', async () => {
    renderCard();
    await userEvent.click(screen.getByText('Goku').closest('div')!);
  });

  it('applies blue badge for Z Fighters affiliation', () => {
    renderCard({ affiliation: 'Z Fighters' });
    const badge = screen.getByText('Z Fighters');
    expect(badge.className).toContain('bg-blue-600');
  });

  it('applies purple badge for Frieza Force affiliation', () => {
    renderCard({ affiliation: 'Frieza Force' });
    const badge = screen.getByText('Frieza Force');
    expect(badge.className).toContain('bg-purple-600');
  });
});
