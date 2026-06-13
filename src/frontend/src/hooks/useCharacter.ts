import { useEffect, useState } from 'react';
import { API_BASE } from '../config';
import type { Character } from '../types/character';

export function useCharacter(id: string | undefined) {
  const [character, setCharacter] = useState<Character | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;

    const controller = new AbortController();

    fetch(`${API_BASE}/characters/${id}`, { signal: controller.signal })
      .then((res) => {
        if (!res.ok) throw new Error(res.status === 404 ? 'Personaje no encontrado' : `Error ${res.status}`);
        return res.json() as Promise<Character>;
      })
      .then(setCharacter)
      .catch((err: Error) => {
        if (err.name !== 'AbortError') setError(err.message);
      })
      .finally(() => setLoading(false));

    return () => controller.abort();
  }, [id]);

  return { character, loading, error };
}
