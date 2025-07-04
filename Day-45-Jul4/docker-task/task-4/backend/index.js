const express = require('express');
const app = express();
const PORT = 5000;

app.get('/hello', (req, res) => {
  res.json({ message: 'Hello from backend!' });
});

app.listen(PORT, () => {
  console.log(`Backend running on port ${PORT}`);
});
