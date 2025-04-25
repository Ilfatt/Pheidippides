<template>
  <header class="header">
    <router-link to="/">
      <div class="header__logo">
        <img
            class="logo__icon"
            src="@/assets/img/pheidippidesIcon.svg"
            alt="logo">
        <p>PHEIDIPPIDES</p>
      </div>
    </router-link>

    <div class="header__nav-menu">
      <router-link
          class="nav-menu__link"
          to="/schedule"
          active-class="active"
      >
        Расписание
      </router-link>
      <router-link
          class="nav-menu__link"
          to="/incident"
          active-class="active"
      >
        Инциденты
      </router-link>
      <router-link
          class="nav-menu__link"
          to="/profile"
          active-class="active"
      >
        Профиль
      </router-link>
      <button
          class="nav-menu__logout"
          @click="logout">
        Выйти
      </button>
    </div>
  </header>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore.js'

const router = useRouter()
const auth = useAuthStore()

const logout = () => {
  auth.logout()
  router.push('/login')
}
</script>

<style scoped>
.header {
  background-color: var(--primary-background-color);
  padding: 16px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header__logo {
  display: flex;
  align-items: center;
  gap: 14px;

  text-align: center;
  font-size: 32px;
  color: var(--primary-text-color);
  text-decoration: none;
}

.logo__icon {
  width: 50px;
  height: 50px;
}

.header__nav-menu {
  display: flex;
  gap: 32px;
  align-items: center;
}

.nav-menu__link {
  padding: 6px 14px;

  border-radius: 10px;
  color: var(--primary-text-color);
  background-color: inherit;
  font-weight: bold;
  text-decoration: none;

  transition: color 0.5s;
}

.nav-menu__link:hover,
.active  {
  background-color: var(--highlight-background-color);

  transition: color 0.5s;
}

.logout {
  background: none;
  border: none;
  color: #dc2626;
  cursor: pointer;
  font: inherit;
  padding: 0;
}

.logout:hover {
  color: #b91c1c;
}
</style>
