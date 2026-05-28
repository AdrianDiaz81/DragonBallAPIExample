import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { SearchBar } from './SearchBar';

describe('SearchBar', () => {
  it('renders the input with placeholder', () => {
    render(<SearchBar value="" onChange={() => {}} />);
    expect(screen.getByPlaceholderText('Buscar personaje...')).toBeInTheDocument();
  });

  it('shows the current value', () => {
    render(<SearchBar value="Goku" onChange={() => {}} />);
    expect(screen.getByDisplayValue('Goku')).toBeInTheDocument();
  });

  it('calls onChange with typed text', async () => {
    const onChange = vi.fn();
    render(<SearchBar value="" onChange={onChange} />);

    await userEvent.type(screen.getByPlaceholderText('Buscar personaje...'), 'Goku');

    expect(onChange).toHaveBeenCalledTimes(4);
    expect(onChange).toHaveBeenLastCalledWith('u');
  });
});
