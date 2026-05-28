interface Props {
  value: string;
  onChange: (value: string) => void;
}

export function SearchBar({ value, onChange }: Props) {
  return (
    <div className="relative w-full max-w-md">
      <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">🔍</span>
      <input
        type="text"
        placeholder="Buscar personaje..."
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="w-full bg-[#1f2937] border border-[#374151] text-white placeholder-gray-500
                   rounded-xl pl-10 pr-4 py-2.5 outline-none
                   focus:border-orange-500 focus:ring-1 focus:ring-orange-500 transition-colors"
      />
    </div>
  );
}
