import { renderHook, waitFor } from '@testing-library/react';
import { mockCharacters } from '../test/fixtures';
import { useCharacters } from './useCharacters';

describe('useCharacters', () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it('returns loading=true initially', () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue({
      ok: true,
      json: () => new Promise(() => {}),
    } as Response);

    const { result } = renderHook(() => useCharacters());

    expect(result.current.loading).toBe(true);
    expect(result.current.characters).toEqual([]);
    expect(result.current.error).toBeNull();
  });

  it('returns characters on successful fetch', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue({
      ok: true,
      json: () => Promise.resolve(mockCharacters),
    } as Response);

    const { result } = renderHook(() => useCharacters());

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.characters).toEqual(mockCharacters);
    expect(result.current.error).toBeNull();
  });

  it('returns error when fetch fails with non-ok response', async () => {
    vi.spyOn(globalThis, 'fetch').mockResolvedValue({
      ok: false,
      status: 500,
      json: () => Promise.resolve({}),
    } as Response);

    const { result } = renderHook(() => useCharacters());

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.error).toBe('Error 500');
    expect(result.current.characters).toEqual([]);
  });

  it('returns error when fetch throws a network error', async () => {
    vi.spyOn(globalThis, 'fetch').mockRejectedValue(new Error('Network error'));

    const { result } = renderHook(() => useCharacters());

    await waitFor(() => expect(result.current.loading).toBe(false));

    expect(result.current.error).toBe('Network error');
    expect(result.current.characters).toEqual([]);
  });
});
