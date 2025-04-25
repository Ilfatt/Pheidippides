<template>
  <div class="modal-overlay" @click.self="close">
    <div class="modal">
      <h2>Редактировать профиль</h2>
      <div class="modal__field">
        <label>Телефон (не редактируется)</label>
        <input type="text" :value="model.phoneNumber" disabled />
      </div>
      <div class="modal__field">
        <label>Email</label>
        <input type="email" v-model="model.email" />
      </div>
      <div class="modal__field">
        <label>Yandex ID</label>
        <div id="yandex-login-button"></div>
        <p v-if="model.yandexOAuthToken">{{ model.yandexOAuthToken }}</p>
      </div>

      <div class="modal__actions">
        <button @click="save">Сохранить</button>
        <button @click="close">Отмена</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import {onMounted, reactive, watch} from 'vue'

const props = defineProps({
  data: Object,
})
const emit = defineEmits(['close', 'save'])

const model = reactive(JSON.parse(JSON.stringify(props.data))) // глубокая копия

const close = () => emit('close')
const save = () => emit('save', model)

watch(model, (newValue) => {
  console.log(newValue)
})

onMounted(() => {
  const script = document.createElement('script');

  window.YaAuthSuggest.init({
      client_id: 'f319c975f0254125a1e81e97e0fe421f',
      response_type: 'token',
      // redirect_uri: window.location.origin,
    },
    window.location.origin, {
      view: 'button',
      parentId: 'yandex-login-button',
      buttonView: 'main',
      buttonTheme: 'light',
      buttonSize: 'm',
      buttonBorderRadius: 0
    })
    .then(result => result.handler())
    .then(data => {
      alert(data)
      console.log('access_token:', data.access_token);
      model.yandexOAuthToken = data.access_token;
    })
    .catch(error => {
      console.error('Ошибка при получении токена:', error);
    });

  document.head.appendChild(script);
});
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  justify-content: center;
  align-items: center;
}

.modal {
  background: white;
  border-radius: 12px;
  padding: 24px;
  width: 400px;
}

.modal__field {
  display: flex;
  flex-direction: column;
  margin-bottom: 16px;
}

.modal__actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}
</style>
