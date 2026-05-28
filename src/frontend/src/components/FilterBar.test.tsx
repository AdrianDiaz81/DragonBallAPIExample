import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { FilterBar } from './FilterBar';

const defaultProps = {
  races: ['Saiyan', 'Namekian', 'Human'],
  affiliations: ['Z Fighters', 'Frieza Force'],
  selectedRace: '',
  selectedAffiliation: '',
  onRaceChange: vi.fn(),
  onAffiliationChange: vi.fn(),
};

describe('FilterBar', () => {
  it('renders all race options', () => {
    render(<FilterBar {...defaultProps} />);
    expect(screen.getByRole('option', { name: 'Saiyan' })).toBeInTheDocument();
    expect(screen.getByRole('option', { name: 'Namekian' })).toBeInTheDocument();
    expect(screen.getByRole('option', { name: 'Human' })).toBeInTheDocument();
  });

  it('renders all affiliation options', () => {
    render(<FilterBar {...defaultProps} />);
    expect(screen.getByRole('option', { name: 'Z Fighters' })).toBeInTheDocument();
    expect(screen.getByRole('option', { name: 'Frieza Force' })).toBeInTheDocument();
  });

  it('calls onRaceChange when a race is selected', async () => {
    const onRaceChange = vi.fn();
    render(<FilterBar {...defaultProps} onRaceChange={onRaceChange} />);

    await userEvent.selectOptions(screen.getByDisplayValue('Todas las razas'), 'Saiyan');

    expect(onRaceChange).toHaveBeenCalledWith('Saiyan');
  });

  it('calls onAffiliationChange when an affiliation is selected', async () => {
    const onAffiliationChange = vi.fn();
    render(<FilterBar {...defaultProps} onAffiliationChange={onAffiliationChange} />);

    await userEvent.selectOptions(screen.getByDisplayValue('Todas las afiliaciones'), 'Z Fighters');

    expect(onAffiliationChange).toHaveBeenCalledWith('Z Fighters');
  });

  it('reflects the selected race value', () => {
    render(<FilterBar {...defaultProps} selectedRace="Saiyan" />);
    expect(screen.getByDisplayValue('Saiyan')).toBeInTheDocument();
  });
});
