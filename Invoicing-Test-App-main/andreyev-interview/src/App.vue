<template>
  <h1>Invoicing Application</h1>
  <div id="app" style="width:1000px;">
    <nav>
      <router-link to="/invoices">Invoices</router-link>
      <router-link to="/clients">Clients</router-link>
    </nav>
      <router-view />
  </div>
</template>

<style>
nav {
  padding: 1rem;
  background: #f0f0f0;
}

nav a {
  margin: 0 1rem;
  text-decoration: none;
  color: #333;
}

nav a.router-link-active {
  font-weight: bold;
}
</style>

