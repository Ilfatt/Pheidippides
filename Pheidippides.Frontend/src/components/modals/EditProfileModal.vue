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
        <input
          v-model="model.email"
          type="email"
          placeholder="Введите почту"
        />
      </div>

      <div class="modal__field">
        <div class="field__label">
          <label>Yandex ID</label>
          <button
            v-if="!model.yandexOAuthToken"
            class="label__ya-code"
            @click="openYandexOAuth"
          >
            получить код
          </button>
          <p
            class="label__ya-status"
            v-if="model.yandexOAuthToken"
          >
            ✔ Привязано
          </p>
        </div>

        <input
          v-if="!model.yandexOAuthToken"
          v-model="confirmationCode"
          type="text"
          placeholder="Вставьте код подтверждения"
        />
        <p
          v-if="errorCode && !model.yandexOAuthToken"
          class="error">
          Неверный код
        </p>
        <button
          v-if="confirmationCode && !model.yandexOAuthToken"
          @click="exchangeCodeForToken"
        >
          Подтвердить
        </button>
      </div>

      <div
        v-if="model.yandexOAuthToken"
        class="modal__field"
      >
        <label>ID сценария</label>

        <input
          v-model="model.yandexScenarioId"
          type="text"
          placeholder="Вставьте id сценария"
        />
      </div>

      <div class="modal__actions">
        <button @click="saveConnectionData">Сохранить</button>
        <button @click="emit('close')">Отмена</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import axios from 'axios'
import {updateProfileInfo} from "@/api/profileApi.js";

const props = defineProps({ data: Object })
const emit = defineEmits(['close', 'save'])

const model = reactive(JSON.parse(JSON.stringify(props.data)))

const showCodeInput = ref(false)
const confirmationCode = ref('')
const errorCode = ref(false)

const clientId = 'f319c975f0254125a1e81e97e0fe421f'

const openYandexOAuth = () => {
  const url = `https://oauth.yandex.ru/authorize?response_type=code&client_id=${clientId}`
  window.open(url, '_blank')
  showCodeInput.value = true
}

const exchangeCodeForToken = async () => {
  try {
    const response = await axios.post(
      'https://oauth.yandex.ru/token',
      new URLSearchParams({
        grant_type: 'authorization_code',
        code: confirmationCode.value,
        client_id: clientId,
        client_secret: '300bf868a96043a7ab97b93662aa8716',
      }),
      {
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded',
        },
      }
    )

    const { access_token } = response.data
    model.yandexOAuthToken = access_token
  } catch (err) {
    console.error('Ошибка при обмене кода на токен:', err);
    errorCode.value = true;
  }
}

const saveConnectionData = async () => {
  const profileDataToSend = {
    email: model?.email,
    yandexScenarioId: model?.yandexScenarioId,
    yandexOAuthToken: model?.yandexOAuthToken,
  }

  try {
    await updateProfileInfo(profileDataToSend);

    emit('save');
  } catch (error) {
    console.error(error);
  }
}

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

.field__label {
  display: flex;
  gap: 5px;
  align-items: baseline;
}

.label__ya-code {
  font-size: 12px;

  color: var(--link-color);
}

.label__ya-status {
  color: var(--additional-green);
}

input {
  padding: 5px 10px;

  border-radius: 10px;
  border: 1px solid lightgray;
}

.error {
  font-size: 12px;

  color: var(--additional-red);
}
</style>
