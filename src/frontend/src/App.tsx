import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { CharacterPage } from './pages/CharacterPage';
import { HomePage } from './pages/HomePage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/characters/:id" element={<CharacterPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
